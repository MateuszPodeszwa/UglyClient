# Project Report

## What I looked at

I went through the whole live solution rather than just reading the README and calling it done. I read the startup code in `Program.cs` and `App.cs`. I traced the active route through `ConsoleHost`, `DashboardController`, `DashboardPage`, `DashboardRenderer`, `CommandParser`, and `CommandExecutor`. I checked the service layer in `Services/Api`, the DTO and result model in `Data` and `Utilities/ErrorHandler`, the cross cutting pieces such as the pipelines and logging, the test suite in `BeautifulClient.Tests`, and the mock API in `SensorServer`. I kept `OldProgram.cs` out of the current architecture write up and only used it here as the before picture.

## What this project is trying to do

At heart this is a console based environment control dashboard for sensors, heaters, and fans. The point of it is not just to toggle a few endpoints. The point is to do that in a way that looks and feels like a proper application rather than a student script. The code is trying to prove that even a terminal app can have a real architecture with dependency injection, clear layers, structured logging, tests, and a domain shaped API boundary.

That goal comes through quite clearly in the current code. The app boots through the standard .NET host. The UI runs through controllers and views rather than one endless `while` loop. Commands become typed records. Network work sits behind `IApiService`. Failures come back as `ApiResult` and `Error` rather than exploding all over the place with exceptions. That is the right direction.

## What changed from `OldProgram.cs`

The old file is a very useful contrast because it shows exactly what this refactor is trying to escape. `OldProgram.cs` is one large file of about five hundred and eighty six lines. It mixes menu text, keyboard input, endpoint knowledge, JSON parsing, temperature control logic, retry free HTTP calls, hard coded URIs, and a hard coded API key in one place. It repeats the same loops for fans and heaters more than once. It catches exceptions near the console layer because there is nowhere else for them to go.

The new codebase breaks that apart into real roles. The host bootstraps. `ConsoleHost` routes. Controllers orchestrate. Pages handle interaction. The renderer draws. The parser understands commands. The executor performs mutations. The API layer hides transport details. The DTO layer gives names to the things moving through the system. That is a big step closer to what you would expect from modern .NET work.

If I were explaining it to a friend over a drink I would say the old version felt like a clever prototype that knew how to do the job. The new version feels like someone sat down and asked how to make that job maintainable once more than one person cares about it.

## Why the current shape is good

The best thing here is that the architecture is visible in the code rather than only in comments. You can point at `DashboardController` and say that is the orchestration point. You can point at `DashboardRenderer` and say that is where the terminal output belongs. You can point at `IApiService` and say that is the contract the rest of the app should depend on. That is what good structure looks like. It lets the next person know where to stand before they make changes.

Another good choice is the local first idea in `UniversalApiFacade`. That is a very sensible design for something that wants to talk to real hardware one day but still needs to run without it. The current implementation is only partly there, but the decision itself is sound because it keeps the UI and the command layer free from transport concerns.

The result and error model is also a good move. In this kind of app network failure is normal life, not a freak event. Returning `ApiResult` makes that explicit and pushes the code toward handling failure as a first class path.

## The patterns that matter

The project uses a proper composition root in `Program.cs`. That is a standard and sensible way to keep object wiring in one place. It means the rest of the app can stay focused on behaviour instead of construction.

It uses the Generic Host pattern. That gives you dependency injection, configuration, logging, and typed `HttpClient` support in a way that feels native to modern .NET. For a console app that is exactly the right way to avoid reinventing the bootstrapping story.

It uses an MVC like navigation model. `ConsoleHost` works as a tiny router, controllers implement `IRouter`, and pages implement `IView<TModel>`. That is not full ASP.NET MVC and it does not need to be. It gives the same separation benefits in a much smaller console shaped package.

It uses the Facade pattern through `IApiService` and `UniversalApiFacade`. That choice hides endpoint paths and transport rules from the UI. The controller code reads like domain behaviour rather than transport code.

It uses the Adapter pattern in `LocalAdapter` and `RemoteAdapter`. Both adapt very different sources into the same client facing contract. That is a strong design move even though the local adapter is not finished.

It uses the Result pattern through `ApiResult`, `ApiResult<T>`, and `Error`. That is one of the cleanest parts of the solution because it makes expected failure cheap and explicit.

It uses a Decorator or Pipeline style in `ApiResultPipeline` and `ObjectSetterPipeline`. The API pipeline adds timing and logging without muddying the service methods. The object setter pipeline intercepts DTO property changes in one place. The latter is a bit adventurous because it leans on reflection over captured closure state, but it is clearly trying to centralise cross cutting behaviour rather than scatter it around.

It uses a Layout Decorator in `MainLayout<TModel>`. That is a neat way to share the frame of the console UI without forcing every page to redraw the same wrapper.

It uses a Value Object for `Celcius`. That is the sort of small design choice that quietly improves a codebase because temperature stops being just another naked `double`.

## The SOLID story

Single Responsibility is the strongest principle in the current shape. The dashboard path is split into sensible pieces with much less overlap than the legacy file. `DashboardPage` does not try to talk HTTP. `CommandExecutor` does not try to draw tables. `DashboardRenderer` does not try to parse commands. That is exactly what you want.

Open Closed is there in a practical way rather than a perfect textbook way. You can add new command formats or command actions in a focused place. You still need to touch switches and enums, so it is not plug in magic, but it is controlled and understandable extension.

Liskov Substitution is mostly respected in the interface design and only weakened by implementation completeness. Both adapters satisfy `IApiService` in type terms, but the local one currently refuses most operations. So the abstraction is good while the behavioural symmetry is not complete yet.

Interface Segregation is handled well. Most interfaces are narrow and useful. There is no giant god interface forcing irrelevant methods onto callers.

Dependency Inversion is another strong point. The controllers and command layer depend on abstractions. That is one of the main reasons the test suite is as healthy as it is.

## The .NET choice

The project pins SDK `10.0.0` in `global.json` and allows prerelease SDK use. The main app multi targets `net8.0` and `net10.0`. The tests and the mock server target `net10.0`.

That tells me the project is trying to balance two different goals. One goal is stability and broad compatibility through `net8.0`. The other goal is learning and using the current platform through `net10.0`. For an academic or portfolio style codebase that is a fair and defensible choice. It lets the main app prove it can live on the long term support runtime while still showing that the repo is being developed with the current SDK.

If this were a product with a larger team and a tighter operations story I would probably collapse the whole solution onto one runtime target for a while. If the priority was conservative deployment I would choose `net8.0`. If the priority was staying current and the team was comfortable with it I would choose `net10.0`. Right now the mixed approach makes sense as a learning and transition move.

## The NuGet packages

I am listing every top level package declared by the solution because that is the part you actively maintain. I am not listing every transitive package pulled in underneath them because that would add noise rather than useful design insight.

### `Microsoft.Extensions.Hosting`

This is what makes the console app behave like a proper hosted .NET application. It gives you the host builder, dependency injection, configuration, and lifecycle plumbing. In this repo it is the backbone of `Program.cs` and the reason the rest of the app can stay cleanly constructed.

### `Microsoft.Extensions.Http`

This package gives you the typed `HttpClient` registration model. It is used for `RemoteAdapter` so the base address and API key header are configured once in the composition root rather than hand built in each call.

### `Microsoft.Extensions.Http.Polly`

This is here so the remote client can get retry behaviour through Polly. The project uses it through `AddPolicyHandler` and `HttpPolicies.GetRetryPolicy`. That is a good fit for a client that might talk to an unreliable remote endpoint.

### `Scrutor`

Scrutor is used for assembly scanning and for view decoration. In practice it is registering controllers that implement `IRouter` and decorating `IView<TModel>` with `MainLayout<TModel>`. It saves manual registration code and makes the layout wrapper cleaner.

### `Serilog.Extensions.Hosting`

This package integrates Serilog with the Generic Host. It lets the app replace the default logger stack with structured logging that fits the rest of the host model.

### `Serilog.Settings.Configuration`

This lets Serilog read configuration from `appsettings.json`. It keeps the logging policy out of hard coded setup and makes the sink choices easier to manage.

### `Serilog.Sinks.Console`

This is the console sink for structured logs. In this repo it is used with a warning level threshold so the normal terminal UI is not drowned in log chatter.

### `Serilog.Sinks.File`

This sink writes logs to rolling files under the `logs` folder. That gives you a persistent trace outside the live terminal session which is useful for debugging.

### `Spectre.Console`

This is the core UI library for the terminal dashboard. It is doing the visible heavy lifting through panels, grids, tables, markup, and captured console output in tests. It is one of the most important package choices in the whole solution because it turns the client from a plain console menu into a richer text UI.

### `Spectre.Console.Cli`

This package is referenced but I could not find an active command app or CLI command registration using it in the live code. That means it is either a forward looking dependency or a leftover from an earlier idea. At the moment it looks unused.

### `Microsoft.NET.Test.Sdk`

This is the foundation for running the test project under `dotnet test`. Without it the xUnit tests would not execute inside the normal .NET test toolchain.

### `xunit`

This is the actual test framework. It is used across controller tests, parser tests, renderer tests, page tests, layout tests, and adapter tests. It fits the repo well because the tests are mostly focused, constructor friendly, and use lightweight doubles.

### `xunit.runner.visualstudio`

This gives the test runner integration needed by IDEs and standard .NET tooling. It is the practical glue that makes the xUnit tests show up and run cleanly in normal workflows.

### `Swashbuckle.AspNetCore`

This is used by the mock API server to expose Swagger and Swagger UI. That is helpful for manual endpoint checking and for understanding the remote side of the integration during development.

## What the git history suggests about how the work was managed

This part is an inference from the current branch history rather than something explicitly documented in the code. What the history looks like to me is an iterative refactor done in vertical slices. There are small commits for the host setup, then the service layer, then the result model, then the dashboard interaction pieces, then tests around those pieces. That is much closer to an Agile and incremental style than to a giant one shot rewrite.

The only caveat is that it appears to have happened on a longer lived refactor branch rather than very short lived branches merged rapidly back to trunk. So I would describe the delivery style as iterative and slice based, but not quite the tightest form of trunk based development.

That is still a good project management choice for this sort of refactor. When you are moving from a large legacy file into architecture, small slices are safer because you can keep proving each piece as you go.

## How it compares with industry standard techniques

In the areas that matter most this code is moving toward industry standard practice quite well. The use of dependency injection, typed `HttpClient`, typed options, structured logging, result based error handling, test doubles, and narrow interfaces are all recognisably modern .NET techniques.

It is also more honest than many over designed student projects because the abstractions generally map to real responsibilities. The controller and page split is not there just to sound clever. It actually matches the work the code is doing.

Where it still sits below a polished production standard is in completeness and consistency rather than in raw idea quality. Some abstractions exist but are only partly realised. Some tests no longer match the implementation. Some registrations and packages look unused. The mock server has overlapping state paths. So the project already speaks the right architectural language, but a few sentences are still unfinished.

## What could be better

The biggest thing to improve is coherence between design and implementation. `ConsoleHost` accepts a settings argument and then drops it on the floor by forcing the initial payload to `null`. `MenuRouteAttribute` looks important but is not actually used for routing. `HomePage` is registered and decorated but not part of the active startup path. Those are the sorts of gaps that make maintainers pause and wonder which parts are real and which parts are aspirational.

The local first story also needs a decision. Right now the structure says local first, but the active dashboard path for devices `1` to `3` goes remote in practice because `HardwarePlugService` only accepts sensor IDs `4` to `6`, and the local adapter does not implement heater or fan mutations. That means the architecture promise is ahead of the implementation.

The stateful DTO story is another area that wants finishing. The idea is good. Change tracking plus save on change could become a strong feature. At the moment the failure path is unfinished and sensor save actions are deliberately unimplemented. I would either complete that feature properly or keep it clearly marked as experimental.

The mock API server also wants simplification. It has a singleton environment state and per client environment states living at the same time. That makes the simulation story harder to reason about than it needs to be.

## Security and operational risks

This is already safer than the legacy file because the main client now loads the API key from user secrets rather than hard coding it in source. That is a real improvement and it is worth saying plainly.

There are still a few risks. The mock server uses a very simple in memory API key dictionary. That is fine for a mock service but not enough for a serious deployment story. Swagger is bypassed in the middleware which is convenient for development but not what you would want for a locked down environment. The API key model has authentication but no meaningful authorisation beyond mapping a key to a client ID.

The logging path also deserves care. `ApiResultPipeline` logs returned values and those DTO `ToString` methods return `RawJson`. In a bigger system that could leak more payload detail into logs than you really want. `SerilogQueSink` also stores logs in a static in process queue, which is handy for the dashboard but means log state is shared across the process.

The legacy file is the biggest historical security warning because it contains a hard coded API key and a hard coded remote address. The refactor improves that a lot.

## What is still missing

I would say the main missing pieces are a fully finished local adapter, a settled state persistence story for DTOs, stronger routing payload types, end to end tests that drive the real mock server, and a cleaner single source of truth for device topology and simulation state.

There is also an opportunity to tighten the startup path so every registered abstraction is either actively used or clearly parked for the next stage. That kind of housekeeping is not glamorous, but it is exactly what makes an already good architecture feel reliable.

## Final view

This codebase is good because it has stopped being one clever file and started becoming a system. It has a visible shape. It has named responsibilities. It has tests. It has a real attempt at domain language. It has a better secret handling story than the old version. It is close enough to industry standard practice that the remaining problems are mostly about follow through rather than basic understanding.

If I were carrying this on I would not rip the architecture up again. I would keep this shape, finish the half built edges, and make the implementation line up cleanly with the design that is already there.
