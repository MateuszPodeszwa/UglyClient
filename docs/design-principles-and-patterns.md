# Design Principles And Patterns

## Reading This Document

This document maps the code to recognised engineering principles and patterns, then calls out the places where the implementation still falls short of the design.

The overall conclusion is that the project has a genuinely strong architectural direction, but it is still part way through turning that direction into a fully coherent implementation.

## SOLID Assessment

| Principle | Where It Shows Up | Benefit In This Codebase | Current Limits |
| --- | --- | --- | --- |
| Single Responsibility | `DashboardController` orchestrates, `DashboardPage` handles input flow, `DashboardRenderer` draws output, `CommandParser` parses, `CommandExecutor` mutates | The live dashboard behaviour is split into understandable units rather than one giant console loop | `HomePageController` still contains placeholder style demo data and the mock server has overlapping state responsibilities |
| Open Closed | New command syntaxes can be added in `CommandParser` and new command actions can be added in `CommandExecutor` without changing the view contracts | The dashboard command system is extensible and testable | The command type enum still needs direct switch changes, so extension is controlled rather than fully open ended |
| Liskov Substitution | `LocalAdapter` and `RemoteAdapter` both satisfy `IApiService`, and pages satisfy `IView<TModel>` | The controller layer can remain agnostic to the transport detail | `LocalAdapter` only partially honours the full behavioural contract because many methods always fail |
| Interface Segregation | `IApiService`, `ICommandParser`, `ICommandExecutor`, `IDashboardRenderer`, `IDashboardInputReader`, `IRouter`, and `IView<TModel>` are all narrow | Consumers depend on small, focused interfaces rather than large god interfaces | `Controller.ExecuteAsync(object? payload)` still pushes a very generic payload shape through the routing system |
| Dependency Inversion | Controllers depend on `IApiService` and `IView<TModel>`, not concrete adapters or concrete pages | Testing is straightforward because the suite can stub these abstractions cheaply | Some DI registrations are rough around the edges, especially the service provider build in `AddPageDecorator` |

## Pattern Catalogue

| Pattern | Primary Evidence | Why It Was Chosen | Real Benefit Here | Real Tradeoff Here |
| --- | --- | --- | --- | --- |
| Composition Root | [`Program.cs`](../Program.cs) | Keep startup, configuration, and dependency wiring in one place | Easier maintenance and clearer bootstrapping | The current composition root has a few redundant registrations |
| Generic Host | [`Program.cs`](../Program.cs), [`App.cs`](../App.cs) | Use the standard .NET hosting model even for a console app | Logging, configuration, DI, and HTTP client setup become idiomatic | It is heavier than a raw `Main`, though the tradeoff is worth it |
| MVC like Navigation | [`Utilities/Controller.cs`](../Utilities/Controller.cs), [`UI/ConsoleHost.cs`](../UI/ConsoleHost.cs), [`UI/Pages/IView.cs`](../UI/Pages/IView.cs) | Replace the legacy monolithic menu loop with route oriented interaction | Each controller owns one use case flow and each view owns one render contract | The router is minimal and payload typing is still weak |
| Facade | [`IApiService`](../Services/Api/IApiService.cs), [`UniversalApiFacade`](../Services/Api/UniversalApiFacade.cs) | Hide local versus remote transport detail from the application layer | Controllers and commands talk in domain actions instead of endpoint paths | Fallback logic is repeated method by method and could be centralised further |
| Adapter | [`LocalAdapter`](../Services/Api/Adapters/LocalAdapter.cs), [`RemoteAdapter`](../Services/Api/Adapters/RemoteAdapter.cs) | Translate raw data sources into the client DTO model | Makes local simulation and remote HTTP look the same to the caller | Local behaviour is incomplete and currently asymmetric |
| Template Method Or Base Class Reuse | [`ApiActions`](../Services/Api/Actions/ApiActions.cs) | Centralise common HTTP and JSON handling | Reduces boilerplate in remote calls | Inheritance can hide some coupling and makes non HTTP local usage slightly awkward |
| Result Pattern | [`ApiResult`](../Utilities/ErrorHandler/ApiResult.cs), [`Error`](../Utilities/ErrorHandler/Error.cs) | Represent expected failure without exceptions | The controller and command layers can reason about success and failure uniformly | Failures still rely on caller discipline because `ApiResult<T>.Value` is unsafe when misused |
| Decorator Or Pipeline | [`ApiResultPipeline`](../Utilities/Pipelines/ApiResultPipeline.cs), [`ObjectSetterPipeline`](../Utilities/Pipelines/ObjectSetterPipeline.cs) | Add logging and timing without contaminating domain logic | Good centralisation of cross cutting concerns | `ObjectSetterPipeline` uses reflection on closure state, which is clever but fragile |
| Layout Decorator | [`MainLayout<TModel>`](../UI/Layouts/MainLayout.cs), [`AddPageDecorator`](../Utilities/Extensions/AddTransientDecoratorExtension.cs) | Wrap views with a shared terminal frame | Shared presentation is kept out of page logic | The layout implementation and its tests have drifted apart |
| Typed `HttpClient` With Retry Policy | [`Program.cs`](../Program.cs), [`Utilities/HttpPolicies.cs`](../Utilities/HttpPolicies.cs) | Keep remote API configuration central and resilient | Base address, headers, and retry rules are not duplicated | Retry behaviour is fixed and not yet surfaced as options |
| Value Object | [`Data/Structs/Temperature/Celcius.cs`](../Data/Structs/Temperature/Celcius.cs) | Treat temperature as a domain concept instead of plain `double` | More expressive types and better formatting | Only Celsius is complete, Fahrenheit is still unfinished |
| Test Double Driven Unit Testing | test doubles under [`BeautifulClient.Tests/UI/TestDoubles`](../BeautifulClient.Tests/UI/TestDoubles) | Isolate controllers, renderers, pages, and command logic | Fast tests with low setup cost | There is little true end to end automated coverage against the mock API server |

## Strongest Architectural Choices

### The Dashboard Split

The split between `DashboardController`, `DashboardPage`, `DashboardRenderer`, `CommandParser`, and `CommandExecutor` is the strongest design decision in the repository.

That split gives each class a narrow job:

- the controller owns orchestration
- the page owns key dispatch and modal prompts
- the renderer owns terminal drawing
- the parser turns text into intent
- the executor turns intent into service calls

This is a strong example of both separation of concerns and the Single Responsibility Principle.

### The Domain Level Service Contract

`IApiService` is also a very good choice because the rest of the application never needs to know about URIs, headers, JSON parsing, or HTTP verbs.

That is closer to industry standard application design than directly passing `HttpClient` around.

### The Result Model

Using `ApiResult` and `Error` is a better fit than exception driven control flow for an interactive terminal application where network and device failures are expected states rather than exceptional ones.

### The Test Surface

The project has a useful automated test surface around:

- controller behaviour
- command parsing
- command execution
- console host routing
- rendering output
- layout decoration
- page input handling
- DTO behaviour

That is a serious improvement over the legacy single file style.

## Where The Implementation Is Still Immature

### Navigation Metadata Exists But Is Not Operational

`MenuRouteAttribute` suggests a route metadata model, but the router currently uses direct `Type` resolution only. This means the attribute is documentation today, not routing infrastructure.

### The Local First Story Is Only Half True

The façade pattern is in place, but the local adapter only supports sensor reads and only for IDs `4`, `5`, and `6`.

That means:

- the active dashboard path for device IDs `1` to `3` always falls back to remote
- heater and fan mutations are always remote
- reset is always remote

The design is good. The implementation is incomplete.

### Stateful DTO Persistence Is Not Finished

The stateful DTO idea is interesting because it aims to track changes and allow save on change behaviour. Right now it is only partially realised.

Current issues:

- `Revert()` is not implemented
- sensor `SaveAction` delegates throw
- `SaveOnChangesAsync()` returns success after calling `Revert()` on failure, which is not a coherent completed failure path

### Some Composition Root Choices Are Rough

`AddPageDecorator` builds a service provider during registration and returns it, even though the return value is ignored. That is not the usual recommended pattern for a .NET composition root because it can create duplicated service graphs and confusing lifetime behaviour.

### The Mock Server Is Architecturally Split

The mock server has a good purpose but a muddled state model.

The most important mismatch is that:

- `EnvironmentUpdater` depends on a singleton `EnvironmentState`
- controllers depend on `ClientStateManager`
- `ClientStateManager` creates fresh per client `EnvironmentState` objects

This means the mock server is carrying two state stories at once.

## Comparison With Industry Standard Practice

On the positive side the project already lines up with several modern .NET practices:

- Generic Host for non web application bootstrapping
- centralised DI
- typed options
- typed `HttpClient`
- structured logging
- explicit testing
- narrow interfaces
- meaningful use of records and value objects

Where it still falls below a polished production standard:

- partially unused abstractions
- duplicated constants
- incomplete state persistence workflow
- small but important implementation versus test drift
- unused package reference surface
- mock backend architecture that is more experimental than settled

## Better Options For The Next Refactor Stage

If the project keeps moving forward, the next improvements with the best payoff are:

1. replace `object? payload` with a stronger typed navigation model
2. decide whether `MenuRouteAttribute` is real routing metadata or just remove it
3. make device topology configurable in one place
4. either finish the local adapter properly or make it an explicit optional capability
5. finish `StatefulDto` save and revert semantics before depending on that feature more widely
6. simplify the mock server into one clear state model
7. remove or justify unused registrations and unused package references

## Summary

BeautifulClient is not just a prettier version of the legacy file. It is a codebase with a recognisable architecture and good direction. The main work left is not inventing a better structure. The main work left is finishing the structure that already exists and removing the places where the implementation still does not fully honour it.
