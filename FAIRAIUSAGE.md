# Fair AI Usage Disclosure

This document provides full transparency about the use of artificial intelligence (AI) in the development of BeautifulClient.

## Purpose

In the spirit of transparency and ethical software development, this file discloses all instances where AI tools have been used in the creation, development, and maintenance of this project.

## AI Tools Used

### 1. **GitHub Copilot**

- **Purpose**: Code completion and generation assistance
- **Usage Areas**:
  - XML documentation comment generation
  - Unit test scaffolding
  - Markdown documents, including readme.md 
- **Human Oversight**: All generated content reviewed

### 2. **Claude (Anthropic)**

- **Purpose**: Architecture design assistance and code review
- **Usage Areas**:
  - Architectural pattern 'best-fitting' suggestions (e.g., MVC-like navigation, Result pattern ...)
  - Refactoring guidance (from `OldProgram.cs` to SOLID-compliant architecture) - No code produced
  - Code review and SOLID principle adherence validation
  - Documentation drafting and improvement

### 3. **ChatGPT / GPT-based Tools**

- **Purpose**: Problem-solving and research assistance
- **Usage Areas**:
  - Research on .NET best practices
  - Debugging assistance for complex issues
  - Explanation of third-party library APIs (Spectre.Console, Polly, Scrutor)
- **Human Oversight**: All suggestions researched and validated independently

## What AI Did NOT Do

To clarify the boundaries of AI involvement:

- **AI did not make architectural decisions** — All architecture decisions were made by developer
- **AI did not design the domain model** — The temperature control system domain was defined by me based on legacy requirements
- **AI did not write tests without review** — All tests were validated for correctness and coverage**
- **AI did not contributed any code into this project** — Every single LOC was produced by me***

---
** As far as my knowledge goes

*** AI was limited only to reviewing, bug fixing and recommending better approaches. 
Program.cs is the only exception where AI produced LOC.

## Specific Examples

### Code Generation

**Example 1: XML Documentation**

AI-assisted generation of XML documentation comments:

```csharp
/// <summary>
/// Retrieves sensor data from the API or local simulation.
/// </summary>
/// <param name="sensorId">The unique identifier of the sensor.</param>
/// <returns>An ApiResult containing SensorData on success.</returns>
public async Task<ApiResult<SensorData>> GetSensorDataAsync(string sensorId)
```

**Human input**: Method signature and intent  
**AI contribution**: Documentation structure and wording  
**Human review**: Verified accuracy and completeness

**Example 2: Unit Test Scaffolding**

AI-generated test structure:

```csharp
[Fact]
public void NavigationResult_GoTo_SetsNextRouteCorrectly()
{
    // Arrange
    var payload = new object();
    
    // Act
    var result = NavigationResult.GoTo<DashboardController>(payload);
    
    // Assert
    Assert.Equal(typeof(DashboardController), result.NextRoute);
    Assert.Equal(payload, result.Payload);
}
```

**Human input**: Test scenario and expected behavior  
**AI contribution**: AAA pattern structure  
**Human review**: Assertion logic validated

### Architecture and Design

**Example 3: Patterns Implementation**

I performed conversation with AI giving my opinion and receiving a feedback regarding the best,
possible pattern and approach for given problem. It was more like a discussion, or meeting.

Based on the result of that discussion, I produced initial LOCs and asked AI for 'professional review'.
```csharp
public readonly record struct ApiResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error Error { get; }
}
```

**Human input**: Initial idea, lines of code and logic draft implementation  
**AI contribution**: Provided suggestions if necessary and recommendations.  
**Human decision**: Adopted pattern after evaluation of alternatives, created by hand

### Documentation

**Example 4: README Structure**

AI-drafted sections of this README and other documentation files.

**Human input**: Project structure, features, and architectural details  
**AI contribution**: Markdown formatting, section organization, badge suggestions, general text formatting  
**Human review**: All content verified for accuracy; sections rewritten where needed

## Principles of AI Use

I follow these principles when using AI tools:

TAVEE? 

1. **Transparency** — Disclose AI usage openly (this document)
2. **Accountability** — I am responsible for all code
3. **Validation** — All AI output is reviewed, tested, and validated
4. **Education** — Use AI to learn, not to replace understanding
5. **Ethics** — Ensure AI-generated code respects licensing and intellectual property.. :/

## Questions and Concerns

If you have questions or concerns about AI usage in this project:

- Open a GitHub issue with the `ai-ethics` label
- Contact me directly via GitHub
- Refer to [CONTRIBUTING.md](CONTRIBUTING.md) for code review standards

## Updates to This Document

This document will be updated whenever:

- New AI tools are adopted
- AI usage patterns change significantly
- The community requests additional transparency

**Last updated**: 2026-04-06

---

**Acknowledgment**: This document itself was drafted with AI assistance (Claude) and reviewed/edited by me as the project author.
