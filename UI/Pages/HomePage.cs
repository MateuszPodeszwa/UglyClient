using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using Spectre.Console;

namespace BeautifulClient.UI.Pages;

public sealed class HomePage : IView<UserDashboardModel>
{
    public Task<NavigationResult> ReturnAsync(UserDashboardModel model)
    {   
        AnsiConsole.MarkupLine($"Welcome, [green]{model.Username}[/]! (ID: {model.UserId})");
        AnsiConsole.WriteLine($"Temperature: [{model.Temperature}]");
        AnsiConsole.WriteLine($"Sensor ID: [{model.SensorId}]");
        AnsiConsole.MarkupLine($"Status: {model.Status}");
        AnsiConsole.MarkupLine($"Payload: {model.Payload}");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Choose the sensor id")
                .AddChoices(1, 2, 3, 4, 5, 6, 7, 8));

        Type? nextRoute = choice switch
        {
            _ => typeof(HomePageController)
        };

        return Task.FromResult(new NavigationResult(nextRoute, choice));
    }
}