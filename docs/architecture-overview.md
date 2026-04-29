# Architecture Overview

## Scope

This document describes the current architecture of the live solution and excludes `OldProgram.cs` from the runtime model.

The active solution is made of three runtime parts:

| Area | Purpose | Key Entry Points |
| --- | --- | --- |
| [`BeautifulClient`](../BeautifulClient.csproj) | Main console dashboard application | [`Program.cs`](../Program.cs), [`App.cs`](../App.cs) |
| [`BeautifulClient.Tests`](../BeautifulClient.Tests/BeautifulClient.Tests.csproj) | Automated tests for the console application | test classes under [`BeautifulClient.Tests`](../BeautifulClient.Tests) |
| [`SensorServer`](../SensorServer/SensorServer.csproj) | Mock ASP.NET Core API used by the client | [`SensorServer/Program.cs`](../SensorServer/Program.cs) |

## Primary Goal

The current codebase is trying to present an interactive terminal dashboard for environmental control and monitoring. The console client reads sensor temperatures and controls heaters and fans. It prefers a local adapter first, then falls back to a remote HTTP API.

The current user facing route starts directly on the live dashboard rather than the older menu driven flow.

## Runtime Entry Path

The startup path in the main application is:

1. [`Program.cs`](../Program.cs) builds a `HostApplicationBuilder`
2. configuration is loaded from `appsettings.json` and user secrets
3. logging, HTTP, controllers, pages, layouts, renderers, commands, and pipelines are registered with DI
4. [`App.RunAsync`](../App.cs) resolves and starts [`ConsoleHost`](../UI/ConsoleHost.cs)
5. `ConsoleHost` resolves a controller type from DI and loops until `NavigationResult.NextRoute` becomes `null`
6. the current application entry route is [`DashboardController`](../UI/Controllers/DashboardController.cs)

## Layer Map

| Layer | Main Files | Responsibility |
| --- | --- | --- |
| Composition root | [`Program.cs`](../Program.cs) | Builds the host, configures DI, HTTP, Serilog, options, and page decorators |
| Application shell | [`App.cs`](../App.cs), [`UI/ConsoleHost.cs`](../UI/ConsoleHost.cs) | Starts the app and keeps the controller navigation loop running |
| UI navigation | [`Utilities/Controller.cs`](../Utilities/Controller.cs), [`Utilities/IRouter.cs`](../Utilities/IRouter.cs), [`Data/Records/NavigationResult.cs`](../Data/Records/NavigationResult.cs) | Defines controller contracts and route hand off |
| Controllers | [`UI/Controllers`](../UI/Controllers) | Orchestrate use cases and prepare view models |
| Views and layouts | [`UI/Pages`](../UI/Pages), [`UI/Layouts`](../UI/Layouts) | Render the terminal UI and return the next navigation decision |
| Dashboard interaction | [`UI/Commands`](../UI/Commands), [`UI/Rendering`](../UI/Rendering) | Parse commands, execute mutations, draw the dashboard, read input |
| Service layer | [`Services/Api`](../Services/Api) | Exposes the domain level API contract and local first façade |
| Data and results | [`Data`](../Data), [`Utilities/ErrorHandler`](../Utilities/ErrorHandler) | DTOs, temperature value types, result wrappers, errors, and command records |
| Cross cutting concerns | [`Utilities/Pipelines`](../Utilities/Pipelines), [`Utilities/SerilogQueSink.cs`](../Utilities/SerilogQueSink.cs), [`Utilities/HttpPolicies.cs`](../Utilities/HttpPolicies.cs) | Logging, retry behaviour, mutation interception, and in process log capture |
| Mock backend | [`SensorServer`](../SensorServer) | Simulates the remote API used by the client |

## Current Active Flow

The active dashboard loop is the most important runtime path.

1. `App` calls `ConsoleHost.Host<DashboardController>()`
2. `ConsoleHost` resolves `DashboardController` from DI
3. `DashboardController.ExecuteAsync` receives the last `ParsedCommand`
4. if the command is `Quit` it returns `NavigationResult(null)`
5. otherwise it optionally delegates mutations to `ICommandExecutor`
6. it fetches sensors, heaters, and fans in parallel through `IApiService`
7. it builds a `DashboardModel`
8. it calls `IView<DashboardModel>.ReturnAsync`
9. `DashboardPage` renders the current snapshot, reads the next key, and returns another `NavigationResult`
10. `ConsoleHost` loops again with the returned route and payload

## Important Current Components

### Composition Root

[`Program.cs`](../Program.cs) is a classical composition root built on `Host.CreateApplicationBuilder`.

It does these things in one place:

- forces the environment name to `Development`
- adds user secrets explicitly from the executing assembly
- configures a typed `HttpClient` for [`RemoteAdapter`](../Services/Api/Adapters/RemoteAdapter.cs)
- attaches a Polly retry policy through [`HttpPolicies`](../Utilities/HttpPolicies.cs)
- registers the application services and pipelines
- scans controllers from the current assembly with Scrutor
- decorates page views with [`MainLayout<TModel>`](../UI/Layouts/MainLayout.cs)

### Navigation Shell

[`ConsoleHost`](../UI/ConsoleHost.cs) is the route loop. It resolves controllers by concrete `Type`, calls `ExecuteAsync`, and uses `NavigationResult` to decide the next step.

This is the closest thing in the codebase to an MVC router, but it is a minimal one. It does not inspect route metadata and it does not use the `MenuRouteAttribute` at runtime.

### Dashboard Use Case

[`DashboardController`](../UI/Controllers/DashboardController.cs) is the centre of the active application. It:

- decides when to exit
- decides whether a payload is only a refresh or a mutation
- calls the command executor for mutations
- fetches device state in parallel
- converts service results into a renderable dashboard model

### Dashboard View Split

The dashboard UI is intentionally split between:

- [`DashboardPage`](../UI/Pages/DashboardPage.cs) for input flow and modal prompts
- [`DashboardRenderer`](../UI/Rendering/DashboardRenderer.cs) for Spectre.Console output

That split is one of the strongest design decisions in the codebase because it keeps input handling and drawing separate.

### Local First Service Boundary

[`IApiService`](../Services/Api/IApiService.cs) is the domain contract. [`UniversalApiFacade`](../Services/Api/UniversalApiFacade.cs) tries [`LocalAdapter`](../Services/Api/Adapters/LocalAdapter.cs) first and falls back to [`RemoteAdapter`](../Services/Api/Adapters/RemoteAdapter.cs).

The contract is useful because controllers and commands never need to know whether the data came from local simulation or remote HTTP.

### Result And Error Model

The code uses:

- [`ApiResult`](../Utilities/ErrorHandler/ApiResult.cs)
- [`ApiResult<T>`](../Utilities/ErrorHandler/ApiResult.cs)
- [`Error`](../Utilities/ErrorHandler/Error.cs)

This is a consistent result pattern that keeps expected failures out of normal exception driven control flow.

## Current Architectural Strengths

The strongest parts of the design are:

- a clear composition root
- a recognisable controller and view navigation model
- a clean domain level API interface
- a local first façade that hides transport details from the UI
- a reusable result model for expected failures
- a separate rendering service for the Spectre.Console dashboard
- a substantial automated test suite around commands, controllers, pages, rendering, and data objects

## Important Current Gaps And Mismatches

Future maintainers should know these issues up front because they affect how much of the design is already realised.

| Area | Current State | Evidence |
| --- | --- | --- |
| Initial settings payload | `ConsoleHost.Host` accepts `settings` but does not pass it to the first controller. The local variable is hard set to `null`. | [`UI/ConsoleHost.cs`](../UI/ConsoleHost.cs) |
| Route metadata | `MenuRouteAttribute` exists but is descriptive only. The runtime loop does not read it. | [`Utilities/Attributes/MenuRouteAttribute.cs`](../Utilities/Attributes/MenuRouteAttribute.cs), [`UI/ConsoleHost.cs`](../UI/ConsoleHost.cs) |
| Home route | `HomePage` and `HomePageController` are registered but the app starts directly at `DashboardController`. | [`Program.cs`](../Program.cs), [`App.cs`](../App.cs) |
| Local first behaviour | The dashboard uses device IDs `1` to `3`, while `HardwarePlugService` only succeeds for sensor IDs `4` to `6`. In practice the dashboard sensor reads always fall back to remote. | [`UI/Controllers/DashboardController.cs`](../UI/Controllers/DashboardController.cs), [`Services/Hardware/HardwarePlugService.cs`](../Services/Hardware/HardwarePlugService.cs) |
| Local adapter completeness | `LocalAdapter` only implements sensor reads. Heater, fan, and reset operations all return `LocalApi.Fail`. | [`Services/Api/Adapters/LocalAdapter.cs`](../Services/Api/Adapters/LocalAdapter.cs) |
| Stateful save path | `StatefulDto.SaveOnChangesAsync` has an unfinished revert path and sensor `SaveAction` delegates throw `NotImplementedException`. | [`Data/StatefulDto.cs`](../Data/StatefulDto.cs), [`Services/Api/Adapters/RemoteAdapter.cs`](../Services/Api/Adapters/RemoteAdapter.cs) |
| DI registration style | `AddPageDecorator` builds a service provider during registration, which is usually an anti pattern in a composition root. | [`Utilities/Extensions/AddTransientDecoratorExtension.cs`](../Utilities/Extensions/AddTransientDecoratorExtension.cs) |
| Hard coded topology | Device count `3` is duplicated in the dashboard controller, command executor, models, tests, and server seed data. | repository search across `DeviceCount`, `ExpectedDeviceCount`, and server seed loops |
| Unused package | `Spectre.Console.Cli` is referenced but no command app or CLI entry point uses it. | [`BeautifulClient.csproj`](../BeautifulClient.csproj) |
| Mock server state model | `SensorServer` keeps both a singleton `EnvironmentState` and separate per client `EnvironmentState` instances in `ClientStateManager`, which splits the simulation story. | [`SensorServer/Program.cs`](../SensorServer/Program.cs), [`SensorServer/Models/EnvironmentUpdater.cs`](../SensorServer/Models/EnvironmentUpdater.cs), [`SensorServer/Models/ClientState.cs`](../SensorServer/Models/ClientState.cs) |

## Mock Server Architecture In Practice

The mock API is not just a thin controller layer. It contains:

- API key middleware
- controller endpoints for sensors, heaters, fans, environment reset, and state views
- per client state management
- background services for cleanup and monitoring
- a separate singleton `EnvironmentState` updated in the background

That gives the client a meaningful integration target, but the server also contains the most architectural drift in the repository.

The two main server tensions are:

1. `ClientStateManager` creates fresh per client `EnvironmentState` objects while the app also registers a singleton `EnvironmentState`
2. the seeding path and the background updater operate on the singleton state, while the controllers mostly operate on per client state

## How To Read The Rest Of The Docs

Use the companion documents like this:

- read [Design Principles And Patterns](design-principles-and-patterns.md) if you want the reasoning behind the code shape
- read [UML Diagrams](uml-diagrams.md) if you want the component and sequence views
- read [Project Report](project-report.md) if you want the broad narrative, dependency catalogue, and legacy comparison
