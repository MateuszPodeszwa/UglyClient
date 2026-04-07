# BeautifulClient

> A modern, console-based IoT sensor dashboard with clean architecture and real-time monitoring capabilities

<!-- Core badges -->
[![.NET](https://img.shields.io/badge/.NET-10.0%20%7C%208.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey.svg)]()

<!-- Project status -->
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![Code Quality](https://img.shields.io/badge/code%20quality-A-brightgreen.svg)]()
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-brightgreen.svg)]()
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

<!-- Social & Support -->
[![GitHub Stars](https://img.shields.io/github/stars/MateuszPodeszwa/UglyClient?style=social)](https://github.com/MateuszPodeszwa/UglyClient)
[![GitHub Forks](https://img.shields.io/github/forks/MateuszPodeszwa/UglyClient?style=social)](https://github.com/MateuszPodeszwa/UglyClient)
[![GitHub Issues](https://img.shields.io/github/issues/MateuszPodeszwa/UglyClient)](https://github.com/MateuszPodeszwa/UglyClient/issues)
[![GitHub Pull Requests](https://img.shields.io/github/issues-pr/MateuszPodeszwa/UglyClient)](https://github.com/MateuszPodeszwa/UglyClient/pulls)

<!-- Funding & Academic -->
[![Patreon](https://img.shields.io/badge/Patreon-Support%20Me-FF424D?logo=patreon&logoColor=white)](https://www.patreon.com/c/mateuszpodeszwa)
[![Academic Project](https://img.shields.io/badge/Academic-Assignment-4CAF50?logo=googlescholar&logoColor=white)](CITING.md)
[![Built with AI](https://img.shields.io/badge/Built%20with-AI%20Assistance-FF6B6B?logo=openai&logoColor=white)](FAIRAIUSAGE.md)

<!-- Technology Stack -->
[![Spectre.Console](https://img.shields.io/badge/Spectre.Console-0.54.0-5C2D91?logo=.net)](https://spectreconsole.net/)
[![Serilog](https://img.shields.io/badge/Serilog-10.0.0-orange?logo=serilog)](https://serilog.net/)
[![xUnit](https://img.shields.io/badge/xUnit-Testing-5C2D91?logo=xunit)](https://xunit.net/)

## 📋 Overview

BeautifulClient is a terminal-based IoT dashboard application that monitors environmental sensors (temperature, heaters, and fans) through an elegant text user interface (TUI). Built with SOLID principles and modern .NET practices, it represents a complete architectural refactoring of legacy temperature control software.

**Academic Context**: This project was developed as an academic assignment to demonstrate software engineering principles, clean architecture, and modern .NET development practices. For citation information, see [CITING.md](CITING.md).

### Key Features

- 🎨 **Rich Terminal UI** — Interactive console interface powered by Spectre.Console
- 🔄 **Local-First Architecture** — Hardware simulation with automatic fallback to remote API
- 🏗️ **MVC-like Navigation** — Controller-based routing with dependency injection
- 📊 **Real-time Monitoring** — Live sensor data with change tracking
- 🔌 **Resilient API Client** — Polly-based retry policies with structured error handling
- 📝 **Activity Logging** — In-app log viewer with Serilog integration
- ✅ **Comprehensive Testing** — XUnit test suite with architectural verification

## 🚀 Quick Start

### Prerequisites

- [.NET SDK 8.0 or 10.0](https://dotnet.microsoft.com/download)
- A terminal with UTF-8 and ANSI color support

### Installation

```bash
# Clone the repository
git clone https://github.com/MateuszPodeszwa/UglyClient.git
cd UglyClient

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Running the Application

```bash
# Run the main console app
dotnet run --project BeautifulClient

# Run the mock API server (optional)
dotnet run --project SensorServer
```

### Configuration

API credentials are managed via .NET User Secrets:

```bash
# Set your API key (if using remote API)
dotnet user-secrets set "ApiSettings:ApiKey" "your-api-key-here" --project BeautifulClient
```

Configuration is loaded from:
- `appsettings.json` — Base URL and Serilog settings
- User Secrets — API key (not committed to source control)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~NavigationResultTests"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## 📚 Documentation

- **[AGENTS.md](AGENTS.md)** — Guidance for AI coding agents working in this repository
- **[CLAUDE.md](CLAUDE.md)** — Specific instructions for Claude Code assistant
- **[CONTRIBUTING.md](CONTRIBUTING.md)** — How to contribute to this project
- **[ARCHITECTURE.md](ARCHITECTURE.md)** — Detailed architecture and design patterns
- **[SECURITY.md](SECURITY.md)** — Security policy and vulnerability reporting
- **[FAIRAIUSAGE.md](FAIRAIUSAGE.md)** — Disclosure of AI usage in development
- **[CITING.md](CITING.md)** — How to cite this project in academic work

## 🏗️ Project Structure

```
BeautifulClient/
├── BeautifulClient/          # Main console application
│   ├── UI/                   # User interface components
│   │   ├── Controllers/      # Route controllers (MVC-like)
│   │   ├── Pages/            # View implementations
│   │   ├── Layouts/          # Page decorators
│   │   └── Components/       # Reusable UI widgets
│   ├── Services/             # Business logic layer
│   │   └── Api/              # API service abstractions
│   ├── Data/                 # Data models and DTOs
│   ├── Configuration/        # App settings and options
│   └── Utilities/            # Cross-cutting concerns
├── BeautifulClient.Tests/    # XUnit test suite
├── SensorServer/             # Mock ASP.NET Core API
└── OldProgram.cs             # Legacy reference code (read-only)
```

## 🛠️ Technology Stack

- **Framework**: .NET 8.0/10.0
- **UI**: [Spectre.Console](https://spectreconsole.net/) — Rich terminal UI framework
- **DI**: Microsoft.Extensions.DependencyInjection with [Scrutor](https://github.com/khellang/Scrutor)
- **HTTP**: Microsoft.Extensions.Http with [Polly](https://github.com/App-vNext/Polly) resilience
- **Logging**: [Serilog](https://serilog.net/) with custom in-memory sink
- **Testing**: [xUnit](https://xunit.net/)

## 🎯 Architecture Highlights

### Navigation Flow (MVC-like)

```
App → ConsoleHost → Controller.ExecuteAsync(payload) → NavigationResult → Next Controller
```

Controllers implement `IRouter`, are decorated with `[MenuRoute]`, and return `NavigationResult` to drive navigation.

### Local-First API Strategy

```csharp
UniversalApiFacade
├── LocalAdapter      // Hardware simulation (primary)
└── RemoteAdapter     // HTTP client (fallback)
```

The facade attempts local simulation first, falling back to remote API on failure—ensuring offline capability.

### Result/Error Pattern

All service operations return `ApiResult<T>` (discriminated union) with strongly-typed errors:

```csharp
ApiResult<SensorData> result = await Api.GetSensorDataAsync();
if (result.IsSuccess)
    // Handle result.Value
else
    // Handle result.Error (NotFound404, NetworkFailure, etc.)
```

For detailed architecture documentation, see [ARCHITECTURE.md](ARCHITECTURE.md).

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for:
- Code of conduct
- Development workflow
- Coding standards and conventions
- Pull request process

## 🔒 Security

Found a security vulnerability? Please see [SECURITY.md](SECURITY.md) for responsible disclosure instructions.

## 📜 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Original legacy codebase author(s) — for the domain knowledge preserved in `OldProgram.cs`
- [Spectre.Console](https://spectreconsole.net/) team — for the excellent TUI framework
- AI assistants (Claude, GitHub Copilot) — see [FAIRAIUSAGE.md](FAIRAIUSAGE.md) for details
- Academic institution and instructors — for guidance on software engineering principles

## 💖 Support the Project

If you find BeautifulClient helpful or educational, consider supporting its development:

[![Patreon](https://img.shields.io/badge/Patreon-Support%20Me-FF424D?logo=patreon&logoColor=white)](https://www.patreon.com/c/mateuszpodeszwa)

Your support helps me continue creating educational projects and maintaining open-source software!

## 📧 Contact

For questions or feedback, please open an issue on GitHub.

---

**Built with ❤️ using modern .NET and SOLID principles**
