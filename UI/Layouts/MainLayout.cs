using BeautifulClient.Data.Records;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities; // Assuming IView is here now based on your logs
using Spectre.Console;

namespace BeautifulClient.UI.Layouts;

public class MainLayout<TModel>(IView<TModel> innerView) : IView<TModel>
{
    public async Task<NavigationResult> ReturnAsync(TModel model)
    {
        AnsiConsole.Clear();

        var header = new Panel(
                new Align(new Markup("[bold]ENVIRONMENT CONTROL DASHBOARD[/]"), HorizontalAlignment.Center))
            .Expand()
            .Border(BoxBorder.Rounded);
        AnsiConsole.Write(header);

        /*var footer = new Panel(logGrid)
            .Header("[grey]Activity Logs[/]")
            .Expand()
            .Border(BoxBorder.Rounded);*/

        // AnsiConsole.Write(footer);

        AnsiConsole.WriteLine();

        return await innerView.ReturnAsync(model);
    }
}