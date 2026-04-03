using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using Spectre.Console;

namespace BeautifulClient.UI.Pages;

public sealed class HomePage : IView<UserDashboardModel>
{
    public Task<(Type? nextRoute, object? payload)> ReturnAsync(UserDashboardModel model)
    {   
        AnsiConsole.MarkupLine($"Welcome, [green]{model.Username}[/]! (ID: {model.UserId})");
        AnsiConsole.WriteLine($"Temperature: [{model.Temperature}]");
        AnsiConsole.WriteLine($"Sensor ID: [{model.SensorId}]");
        AnsiConsole.MarkupLine($"Status: {model.Status}");
        AnsiConsole.MarkupLine($"Payload: {model.Payload}");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices("Refresh", "Exit"));

        Type? nextRoute = choice switch
        {
            "Refresh" => typeof(HomePageController), // Route back to the controller
            "Exit" => null,
            _ => null
        };

        return Task.FromResult<(Type? nextRoute, object? payload)>((nextRoute, choice));
    }
}