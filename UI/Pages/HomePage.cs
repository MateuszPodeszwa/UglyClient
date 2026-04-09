using BeautifulClient.Data.Records;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using Spectre.Console;

namespace BeautifulClient.UI.Pages;

// Test Class
/// <summary>
/// Abstraction for selecting a sensor ID from the home page.
/// </summary>
public interface IHomePageInput
{
    /// <summary>
    /// Prompts for and returns the selected sensor identifier.
    /// </summary>
    int PromptSensorId();
}

/// <summary>
/// Default <see cref="IHomePageInput"/> implementation backed by <see cref="AnsiConsole"/>.
/// </summary>
public sealed class AnsiConsoleHomePageInput : IHomePageInput
{
    /// <inheritdoc/>
    public int PromptSensorId()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Choose the sensor id")
                .AddChoices(1, 2, 3, 4, 5, 6, 7, 8));
    }
}

public sealed class HomePage : IView<UserDashboardModel>
{
    private readonly IHomePageInput homePageInput;

    /// <summary>
    /// Creates a home page using the default console input implementation.
    /// </summary>
    public HomePage() : this(new AnsiConsoleHomePageInput())
    {
    }

    /// <summary>
    /// Creates a home page with a custom input source.
    /// </summary>
    /// <param name="homePageInput">Input provider used to select the next sensor ID.</param>
    public HomePage(IHomePageInput homePageInput)
    {
        this.homePageInput = homePageInput;
    }

    /// <inheritdoc/>
    public Task<NavigationResult> ReturnAsync(UserDashboardModel model)
    {   
        AnsiConsole.MarkupLine($"Welcome, [green]{model.Username}[/]! (ID: {model.UserId})");
        AnsiConsole.WriteLine($"Temperature: [{model.Temperature}]");
        AnsiConsole.WriteLine($"Sensor ID: [{model.SensorId}]");
        AnsiConsole.MarkupLine($"Status: {model.Status}");
        AnsiConsole.MarkupLine($"Payload: {model.Payload}");
        AnsiConsole.WriteLine();

        int choice = homePageInput.PromptSensorId();

        Type? nextRoute = choice switch
        {
            _ => typeof(HomePageController)
        };

        return Task.FromResult(new NavigationResult(nextRoute, choice));
    }
}
