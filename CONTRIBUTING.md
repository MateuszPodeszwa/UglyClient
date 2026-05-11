# Contributing to BeautifulClient

Thank you for your interest in contributing to BeautifulClient! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Architecture Constraints](#architecture-constraints)
- [Testing Requirements](#testing-requirements)
- [Submitting Changes](#submitting-changes)
- [Documentation](#documentation)

## Code of Conduct

### My Pledge

I am committed to providing a welcoming and inclusive environment for all contributors, regardless of experience level, background, or identity.

### Expected Behavior

- Be respectful and constructive in communication
- Focus on what is best for the project and community
- Show empathy towards other contributors
- Accept constructive criticism gracefully

### Unacceptable Behavior

- Harassment, trolling, or discriminatory language
- Publishing others' private information without permission
- Any conduct that would be inappropriate in a professional setting

## Getting Started

### Prerequisites

- .NET SDK 8.0 or 10.0
- Git
- A code editor (Visual Studio, Rider, or VS Code recommended)
- Familiarity with C# and SOLID principles

### Fork and Clone

```bash
# Fork the repository on GitHub, then clone your fork
git clone https://github.com/YOUR-USERNAME/UglyClient.git
cd UglyClient

# Add upstream remote
git remote add upstream https://github.com/MateuszPodeszwa/UglyClient.git
```

### Build and Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run all tests
dotnet test
```

## Development Workflow

### 1. Create a Feature Branch

```bash
# Sync with upstream
git checkout main
git pull upstream main

# Create a feature branch
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes

- Write clean, well-documented code
- Follow existing code style and conventions
- Add or update tests as needed
- Update documentation for user-facing changes

### 3. Test Your Changes

```bash
# Run the full test suite
dotnet test

# Run the application to verify behavior
dotnet run --project BeautifulClient

# If changes affect the API, also test SensorServer
dotnet run --project SensorServer
```

### 4. Commit Your Changes

```bash
# Stage changes
git add .

# Commit with a descriptive message
git commit -m "Add feature: brief description

- Detailed point 1
- Detailed point 2
- Fixes #issue-number (if applicable)"
```

### 5. Push and Create Pull Request

```bash
# Push to your fork
git push origin feature/your-feature-name

# Create a pull request on GitHub
```

**Note**: As this is an academic assignment project, contributions are welcome for educational purposes. Please ensure any contributions align with academic integrity guidelines.

## Coding Standards

### General Principles

- **SOLID Principles** — All code must follow SOLID design principles
- **DRY (Don't Repeat Yourself)** — Avoid code duplication; extract shared logic
- **Separation of Concerns** — Keep UI, business logic, and data layers distinct

### C# Conventions

- **Naming**:
  - `PascalCase` for classes, methods, properties, and public members
  - `camelCase` for local variables and private fields
  - `_camelCase` for private fields (if not using primary constructors)
  - Descriptive names over abbreviations

- **Code Style**:
  - Use primary constructors where appropriate
  - Enable nullable reference types (`<Nullable>enable</Nullable>`)
  - Prefer expression-bodied members for simple properties/methods
  - Use `var` when the type is obvious from the right-hand side

### Documentation

- **XML Documentation Comments** — Required for all public members:

```csharp
/// <summary>
/// Retrieves sensor data from the API or local simulation.
/// </summary>
/// <param name="sensorId">The unique identifier of the sensor.</param>
/// <returns>An ApiResult containing SensorData on success.</returns>
public async Task<ApiResult<SensorData>> GetSensorDataAsync(string sensorId)
{
    // Implementation
}
```

- **Inline Comments** — Only when necessary to clarify complex logic; prefer self-documenting code

### Error Handling

- Use `ApiResult<T>` pattern for expected failures (domain errors)
- Throw exceptions only for unexpected, exceptional situations
- Never swallow exceptions without logging

## Architecture Constraints

### Non-Negotiable Rules

1. **Do NOT modify `OldProgram.cs`** — It is reference-only legacy code
2. **Preserve the MVC-like navigation pattern** — Controllers → Views → NavigationResult
3. **Use dependency injection** — Avoid `new` for services; register in `Program.cs`
4. **Maintain local-first API strategy** — LocalAdapter before RemoteAdapter
5. **Follow the Result pattern** — Return `ApiResult<T>`, not nulls or exceptions for domain errors

### Preferred Patterns

- **Controllers** extend `Controller`, implement `IRouter`, decorated with `[MenuRoute]`
- **Views** implement `IView<TModel>`, return `NavigationResult`
- **Layouts** are registered via `AddPageDecorator<TViewModel, TPage, TLayout>()`
- **DTOs** extend `StatefulDto<T>` for change tracking
- **Services** registered in DI container, accessed via interfaces

## Testing Requirements

### Test Coverage

- All new public APIs must have unit tests
- Controllers should have integration tests verifying navigation flow
- API adapters should have tests covering success and error paths

### Test Structure

```csharp
public class FeatureTests
{
    [Fact]
    public void Method_Scenario_ExpectedBehavior()
    {
        // Arrange
        var sut = new SystemUnderTest();
        
        // Act
        var result = sut.DoSomething();
        
        // Assert
        Assert.True(result.IsSuccess);
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~NavigationResultTests"

# Generate coverage report (if tooling is set up)
dotnet test /p:CollectCoverage=true
```

## Submitting Changes

### Pull Request Checklist

Before submitting a PR, ensure:

- [ ] Code builds without errors (`dotnet build`)
- [ ] All tests pass (`dotnet test`)
- [ ] New code has appropriate test coverage
- [ ] Public APIs have XML documentation comments
- [ ] Changes follow SOLID principles and project architecture
- [ ] No unintended changes to `OldProgram.cs`
- [ ] Commit messages are clear and descriptive
- [ ] PR description explains what and why (not just how)

### Pull Request Template

```markdown
## Description
Brief description of changes

## Motivation and Context
Why is this change needed? What problem does it solve?

## Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to change)
- [ ] Documentation update

## How Has This Been Tested?
Describe the tests you ran to verify your changes.

## Checklist
- [ ] Code builds without errors
- [ ] All tests pass
- [ ] Added/updated tests for new code
- [ ] Added/updated documentation
- [ ] Follows project coding standards
```

### Review Process

1. I will review your PR within a few days
2. Address any requested changes
3. Once approved, I will merge your PR
4. Your contribution will be included in the next release

## Documentation

When adding features or changing behavior:

- Update relevant sections in `README.md`
- Add/update XML documentation comments in code
- Update `ARCHITECTURE.md` for architectural changes
- Update `AGENTS.md` and `CLAUDE.md` if agent guidance changes
- Add examples if introducing new patterns

## Questions?

If you have questions about contributing:

- Check existing issues and discussions on GitHub
- Open a new issue with the `question` label
- Reach out to me via GitHub

---

**Thank you for contributing to BeautifulClient!** 🎉
