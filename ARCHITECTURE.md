# Architecture Documentation

This document provides a comprehensive overview of BeautifulClient's architecture, design patterns, and technical decisions.

**Academic Context**: This project was developed as an academic assignment to demonstrate software engineering principles, architectural patterns, and the refactoring of legacy code into a clean, maintainable system. For citation information, see [CITING.md](CITING.md).

## Table of Contents

- [Architectural Overview](#architectural-overview)
- [Navigation System](#navigation-system)
- [API Layer](#api-layer)
- [Data Models](#data-models)
- [Cross-Cutting Concerns](#cross-cutting-concerns)
- [Dependency Injection](#dependency-injection)
- [Design Patterns](#design-patterns)

## Architectural Overview

BeautifulClient follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│         (Views, Components)         │
├─────────────────────────────────────┤
│         Application Layer           │
│    (Navigation, Command Handlers,   |
|            Controllers)             │
├─────────────────────────────────────┤
│          Service Layer              │
│   (API Facade, Adapters, Pipelines) │
├─────────────────────────────────────┤
│           Data Layer                │
│       (DTOs, Models, Structs)       │
└─────────────────────────────────────┘
```

### Design Principles

- **SOLID Principles** — Strictly followed throughout the codebase
- **DRY (Don't Repeat Yourself)** — Shared logic is extracted and reused
- **Dependency Inversion** — Dependencies injected via interfaces
- **Single Responsibility** — Each class has one clear purpose

## Navigation System

### MVC-like Pattern

BeautifulClient implements an **MVC-inspired navigation pattern** (please note that the MVC here is exaraggated):

```
App → ConsoleHost → [Controller Loop]
                         ↓
                    Controller.ExecuteAsync(payload)
                         ↓
                    View.RenderAsync(model)
                         ↓
                    NavigationResult (next route + payload)
                         ↓
                    [Resolve next Controller] → Loop continues
```

### Components

#### 1. Controllers

**Location**: `UI/Controllers/`

Controllers orchestrate application flow and handle user interactions.

```csharp
[MenuRoute("dashboard")]
public class DashboardController : Controller
{
    public override async Task<NavigationResult> ExecuteAsync(object? payload)
    {
        // Fetch data via API
        var sensorData = await Api.GetSensorDataAsync();
        
        // Build model
        var model = new DashboardModel(sensorData);
        
        // Delegate to view
        var view = ServiceProvider.GetRequiredService<IView<DashboardModel>>();
        return await view.RenderAsync(model);
    }
}
```

**Key characteristics**:
- Extend `Controller` base class
- `IRouter` interface is already implemented by extending the Controller abstract class,
- Decorated with `[MenuRoute]` attribute
- Access API via `Api` property
- Resolve views from DI container
- Inject relevant `IView<>`

#### 2. Views/Pages

**Location**: `UI/Pages/`

Views are responsible for rendering UI and returning navigation decisions.

```csharp
public class DashboardPage : IView<DashboardModel>
{
    public Task<NavigationResult> RenderAsync(DashboardModel model)
    {
        // Render UI using Spectre.Console
        AnsiConsole.Clear();
        var panel = new Panel($"Temperature: {model.Temperature}");
        AnsiConsole.Write(panel);
        
        // Return navigation decision
        return Task.FromResult(NavigationResult.Stay());
    }
}
```

**Key characteristics**:
- Implement `IView<TModel>` interface
- Use Spectre.Console for rendering
- Return `NavigationResult` to drive navigation
- Stateless — receive model as input

#### 3. Layouts

**Location**: `UI/Layouts/`

Layouts are **decorator wrappers** that add common UI elements (headers, footers, menus) around views.

```csharp
public class MainLayout<TModel> : IView<TModel>
{
    private readonly IView<TModel> _innerView;
    
    public MainLayout(IView<TModel> innerView)
    {
        _innerView = innerView;
    }
    
    public async Task<NavigationResult> RenderAsync(TModel model)
    {
        RenderHeader();
        var result = await _innerView.RenderAsync(model);
        RenderFooter();
        return result;
    }
}
```

**Registration** (in `Program.cs`):

```csharp
builder.AddPageDecorator<DashboardModel, DashboardPage, MainLayout<DashboardModel>>(null);
```

Uses **Scrutor's decorator pattern** to wrap views automatically.

#### 4. NavigationResult

Encapsulates routing decisions:

```csharp
// Navigate to another controller
NavigationResult.GoTo<HomeController>(payload);

// Stay on current page
NavigationResult.Stay();

// Exit application
NavigationResult.Exit();
```

## API Layer

### Local-First Strategy

The API layer follows a **local-first, fallback-to-remote** architecture:

```
IApiService (interface)
    ↓
UniversalApiFacade
    ├── LocalAdapter (primary)
    │   └── HardwarePlugService (hardware simulation)
    └── RemoteAdapter (fallback)
        └── HttpClient + Polly retry policies
```

### UniversalApiFacade

**Location**: `Services/Api/UniversalApiFacade.cs`

The facade attempts local operations first, falling back to remote on failure:

```csharp
public async Task<ApiResult<SensorData>> GetSensorDataAsync()
{
    // Try local first
    var localResult = await _localAdapter.GetSensorDataAsync();
    if (localResult.IsSuccess)
        return localResult;
    
    // Fall back to remote
    return await _remoteAdapter.GetSensorDataAsync();
}
```

### Adapters

Both adapters extend `ApiActions` base class and implement domain operations.

#### LocalAdapter

- Uses `HardwarePlugService` to simulate sensor hardware
- Always available (offline capability)
- Useful for testing and development

#### RemoteAdapter

- HTTP client with Polly retry policies
- Configured with base URL and API key (from User Secrets)
- Retry policy: 3 attempts with exponential backoff

### ApiResult Pattern

All API operations return `ApiResult<T>` — a discriminated union representing success or failure:

```csharp
public readonly record struct ApiResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error Error { get; }
    
    public static ApiResult<T> Success(T value) => new(value);
    public static ApiResult<T> Failure(Error error) => new(error);
}
```

**Usage**:

```csharp
ApiResult<SensorData> result = await Api.GetSensorDataAsync();

if (result.IsSuccess)
{
    Console.WriteLine($"Temperature: {result.Value.Temperature}");
}
else
{
    Console.WriteLine($"Error: {result.Error.Message}");
}
```

### Error Model

Errors are strongly typed with predefined static instances:

```csharp
public record Error
{
    public static readonly Error NotFound404 = new("Resource not found", 404);
    public static readonly Error NetworkFailure = new("Network unavailable", 0);
    public static readonly Error Unauthorized401 = new("Unauthorized", 401);
    // ...
    
    public string Message { get; }
    public int Code { get; }
}
```

## Data Models

### IData Interface

Base contract for all data entities:

```csharp
public interface IData
{
    string Id { get; }
    string RawJson { get; }
}
```

### StatefulDto<T>

Abstract base class providing **change tracking** for DTOs:

```csharp
public abstract class StatefulDto<T> : IData
{
    private readonly Dictionary<string, object?> _changes = new();
    
    protected void SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<TValue>.Default.Equals(field, value))
        {
            field = value;
            _changes[propertyName!] = value;
        }
    }
    
    public bool IsModified => _changes.Count > 0;
    
    public async Task SaveOnChangesAsync()
    {
        if (IsModified)
            await PersistChangesAsync(_changes);
    }
    
    protected abstract Task PersistChangesAsync(Dictionary<string, object?> changes);
}
```

### Domain DTOs

- **SensorData** — Temperature sensor readings
- **HeaterData** — Heater status and settings
- **FanData** — Fan speed and operational state

All extend `StatefulDto<T>` and provide change tracking out of the box.

### Temperature (Celsius)

Custom value type with semantic clarity:

```csharp
public readonly record struct Celcius
{
    public double Value { get; }
    
    public static implicit operator Celcius(double value) => new(value);
    public static implicit operator double(Celcius celsius) => celsius.Value;
    
    public static Celcius Parse(string value) => new(double.Parse(value));
    public static bool TryParse(string value, out Celcius result) { /* ... */ }
    
    // Arithmetic operators
    public static Celcius operator +(Celcius a, Celcius b) => new(a.Value + b.Value);
    public static Celcius operator -(Celcius a, Celcius b) => new(a.Value - b.Value);
    
    // Comparison operators
    public static bool operator >(Celcius a, Celcius b) => a.Value > b.Value;
    public static bool operator <(Celcius a, Celcius b) => a.Value < b.Value;
}
```

**Global alias** (in `Program.cs`):

```csharp
global using celc = BeautifulClient.Data.Structs.Temperature.Celcius;
```

## Cross-Cutting Concerns

### Pipelines

#### ApiResultPipeline

**Location**: `Utilities/Pipelines/ApiResultPipeline.cs`

Wraps API calls with timing and structured logging:

```csharp
public async Task<ApiResult<T>> ExecuteAsync<T>(Func<Task<ApiResult<T>>> apiCall, string operationName)
{
    var stopwatch = Stopwatch.StartNew();
    
    _logger.LogInformation("Starting {Operation}", operationName);
    
    var result = await apiCall();
    
    stopwatch.Stop();
    _logger.LogInformation("{Operation} completed in {ElapsedMs}ms", operationName, stopwatch.ElapsedMilliseconds);
    
    return result;
}
```

#### ObjectSetterPipeline

**Location**: `Utilities/Pipelines/ObjectSetterPipeline.cs`

Intercepts property mutations in `StatefulDto<T>` using **reflection on compiler-generated closures**:

- Captures property name and value from lambda expressions
- Logs changes with structured context
- Useful for debugging and auditing

### Logging

#### Serilog Configuration

- **Console sink** — Warning level and above (keeps console clean)
- **File sink** — All logs to `logs/app-log-{Date}.txt` (daily rolling)
- **SerilogQueSink** — In-memory queue for TUI activity feed

#### SerilogQueSink

**Location**: `Utilities/SerilogQueSink.cs`

Custom Serilog sink that queues log entries in memory:

```csharp
public class SerilogQueSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEntry> _queue = new();
    
    public void Emit(LogEvent logEvent)
    {
        _queue.Enqueue(new LogEntry(logEvent));
    }
    
    public IEnumerable<LogEntry> GetRecentLogs(int count)
    {
        return _queue.TakeLast(count);
    }
}
```

**Usage in MainLayout**:

```csharp
var recentLogs = _serilogSink.GetRecentLogs(10);
foreach (var log in recentLogs)
{
    AnsiConsole.MarkupLine($"[grey]{log.Timestamp}[/] {log.Message}");
}
```

## Dependency Injection

### Service Registration

**Location**: `Program.cs`

All services registered in the DI container:

```csharp
// Singleton services (shared instance)
builder.Services.AddSingleton<App>();
builder.Services.AddSingleton<ConsoleHost>();
builder.Services.AddSingleton<ApiResultPipeline>();
builder.Services.AddSingleton<SerilogQueSink>();

// Transient services (new instance per request)
builder.Services.AddTransient<IApiService, UniversalApiFacade>();
builder.Services.AddTransient<LocalAdapter>();
builder.Services.AddTransient<ICommandParser, CommandParser>();

// Auto-register all IRouter implementations
builder.Services.Scan(scan => scan
    .FromAssemblyOf<HomePageController>()
    .AddClasses(classes => classes.AssignableTo<IRouter>())
    .AsSelf()
    .WithTransientLifetime());

// Decorator registration (Scrutor)
builder.AddPageDecorator<DashboardModel, DashboardPage, MainLayout<DashboardModel>>(null);
```

### Configuration Binding

**Options Pattern** for strongly-typed configuration:

```csharp
// Bind ApiSettings from appsettings.json + User Secrets
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Inject as IOptions<ApiSettings>
public class RemoteAdapter
{
    public RemoteAdapter(IOptions<ApiSettings> options)
    {
        var settings = options.Value;
        // Use settings.BaseUrl, settings.ApiKey
    }
}
```

## Design Patterns

### Patterns Used

1. **MVC (Model-View-Controller)** — Separation of UI, logic, and navigation
2. **Facade Pattern** — `UniversalApiFacade` simplifies API access
3. **Adapter Pattern** — `LocalAdapter` and `RemoteAdapter` adapt different data sources
4. **Decorator Pattern** — Layouts wrap views to add common UI elements
5. **Result Pattern** — `ApiResult<T>` for explicit error handling
6. **Repository Pattern** — `IApiService` abstracts data access
7. **Pipeline Pattern** — `ApiResultPipeline` for cross-cutting concerns
8. **Options Pattern** — Strongly-typed configuration via `IOptions<T>`

### SOLID Principles

- **Single Responsibility** — Each class has one clear purpose
- **Open/Closed** — Extensible via interfaces; closed for modification
- **Liskov Substitution** — Adapters interchangeable via `ApiActions` base
- **Interface Segregation** — Small, focused interfaces (`IView`, `IRouter`, `IData`)
- **Dependency Inversion** — Depend on abstractions (`IApiService`), not concretions

## Historical Context

### Legacy Code

**`OldProgram.cs`** contains the original temperature control system:

- Single file with all logic
- Unsafe code patterns
- No separation of concerns
- Retained as **reference only** (read-only, do not modify)

### Refactoring Goals

BeautifulClient represents a **complete architectural refactoring** with:

- SOLID and DRY principles
- Clean separation of concerns
- Comprehensive error handling
- Testability via dependency injection
- Modern .NET practices (nullable reference types, primary constructors, etc.)

---

**For additional details, refer to:**
- [AGENTS.md](AGENTS.md) — Commands and architectural rules
- [CLAUDE.md](CLAUDE.md) — Extended architecture notes
- [CONTRIBUTING.md](CONTRIBUTING.md) — Development guidelines
