# BeautifulClient Documentation

This folder documents the current `BeautifulClient` solution as it exists today across the main console application, the test project, and the mock API server.

The current architecture documentation deliberately excludes `OldProgram.cs` from the live system description. That legacy file is referenced only in the project report where the refactor is compared with the older style.

## Document Map

- [Architecture Overview](architecture-overview.md) describes the runtime shape of the application, the active execution path, and the important current constraints.
- [Design Principles And Patterns](design-principles-and-patterns.md) maps the codebase to SOLID principles, recurring design patterns, and the places where the current implementation still falls short.
- [UML Diagrams](uml-diagrams.md) contains PlantUML source for structural and behavioural diagrams aimed at future maintainers.
- [Project Report](project-report.md) is the narrative report requested for a hobby programmer audience in plain British English.
- [Black Box Test Report](black-box-test-report.md) is a completed acceptance style matrix written as if the release passed its full end to end checks, as requested.
- [xUnit Justification](xunit-justification.md) explains why xUnit is a sensible testing choice for this solution and what the current suite is doing well.

## Scope And Method

The documentation is based on direct inspection of:

- [`Program.cs`](../Program.cs)
- [`App.cs`](../App.cs)
- [`UI`](../UI)
- [`Services`](../Services)
- [`Data`](../Data)
- [`Utilities`](../Utilities)
- [`BeautifulClient.Tests`](../BeautifulClient.Tests)
- [`SensorServer`](../SensorServer)
- the recent git history on the current branch

## Validation Snapshot

The documentation was written after a repository scan and a local automated test run.

- Actual command run on `2026-04-28` was `dotnet test UglyClient.sln`
- Actual result was `82` passed and `4` failed

The four current automated failures are:

- `BeautifulClient.Tests.UI.ConsoleHostTests.Host_PassesInitialSettingsPayload_ToFirstController`
- `BeautifulClient.Tests.UI.ConsoleHostTests.Host_FollowsRoutesAndPropagatesReturnedPayload`
- `BeautifulClient.Tests.UI.Layouts.MainLayoutTests.ReturnAsync_RendersLayoutAndDelegatesToInnerView`
- `BeautifulClient.Tests.UI.Rendering.DashboardRendererTests.RenderHelp_DisplaysShortcutReferenceAndPresets`

Those failures matter because they expose drift between the intended design and the current implementation. That drift is called out in the architecture notes and in the report so future maintainers can see where the design is strong and where the implementation still needs finishing.

The black box report in this folder is intentionally written as a completed acceptance matrix because that exact presentation was requested.
