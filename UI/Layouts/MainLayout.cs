using BeautifulClient.UI.Components;
using BeautifulClient.UI.Pages; // Assuming IView is here now based on your logs
using Spectre.Console;

namespace BeautifulClient.UI.Layouts;

public class MainLayout<TModel>(IView<TModel> innerView, SerilogQueSink queSink) : IView<TModel>
{
    public async Task<Type?> ReturnAsync(TModel model)
    {
        AnsiConsole.Clear();
        
        var header = new Panel(
                new Align(new Markup("[blue bold]BeautifulClient Dashboard[/]"), HorizontalAlignment.Center))
            .Expand()
            .Border(BoxBorder.Rounded);
        AnsiConsole.Write(header);
        
        var currentLogs = SerilogQueSink.GetLogs();
        
        var logGrid = new Grid().AddColumn(new GridColumn());
        
        foreach (var log in currentLogs)
        {
            logGrid.AddRow(new Markup(log));
        }

        var footer = new Panel(logGrid)
            .Header("[grey]Activity Logs[/]")
            .Expand()
            .Border(BoxBorder.Rounded);
        
        AnsiConsole.Write(footer);
        
        AnsiConsole.WriteLine();
        
        return await innerView.ReturnAsync(model);
    }
}