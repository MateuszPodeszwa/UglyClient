using BeautifulClient.UI.Models;
using BeautifulClient.Utilities;
using BeautifulClient.Utilities.Extensions;
using Spectre.Console;

namespace BeautifulClient.UI.Rendering;

/// <summary>
/// Implements <see cref="IDashboardRenderer"/> using Spectre.Console to produce a rich,
/// webpage-style terminal dashboard for the Environment Control System.
/// </summary>
/// <remarks>
/// All rendering is synchronous and stateless — the same instance may be reused across render cycles.
/// Each public method begins with <c>AnsiConsole.Clear()</c> to produce a clean redraw.
/// </remarks>
public sealed class DashboardRenderer : IDashboardRenderer
{
    private const int MaxHeaterLevel = 5;
    public const string AppTitle = "ENVIRONMENT CONTROL DASHBOARD";

    /// <inheritdoc/>
    public void RenderDashboard(DashboardModel model) // Rendered inside the Layout Wrapper
    {
        RenderHeader(model.RefreshedAt);
        RenderDeviceGrid(model);
        RenderFeedbackBar(model.LastFeedback, model.IsFeedbackError);
        RenderShortcutBar();
    }

    /// <inheritdoc/>
    public void RenderHelp()
    {
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[bold white]{AppTitle} — Keyboard Shortcuts[/]")
            .AddColumn(new TableColumn("[grey]Shortcut[/]").Width(14))
            .AddColumn(new TableColumn("[grey]Mode / Action[/]").Width(20))
            .AddColumn(new TableColumn("[grey]Syntax & Examples[/]"));

        table.AddRow("[cyan]Ctrl+F[/]", "Fan Control", "[grey]<id> on|off  |  all on|off  |  1on 2off 3on[/]");
        table.AddRow("[yellow]Ctrl+H[/]", "Heater Control", "[grey]<id> <0-5>  |  all <0-5>  |  <id>:<level>[/]");
        table.AddRow("[magenta]Ctrl+A[/]","All Devices / Commands", "[grey]fan all on|off  |  heater all <0-5>  |  preset warm|cool|balanced|off[/]");
        table.AddRow("[red]Ctrl+R[/]", "Reset Simulation", "[grey]Prompts for confirmation before posting reset[/]");
        table.AddRow("[blue]Ctrl+L[/]", "Activity Logs", "[grey]Shows the last 10 log entries[/]");
        table.AddRow("[green]r[/]", "Refresh", "[grey]Re-fetch all device data without any mutation[/]");
        table.AddRow("[green]?[/]", "This Help Screen", "");
        table.AddRow("[red]q[/]", "Quit", "");

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[grey]Presets: [/][yellow]warm[/] (heaters 3, fans off)  " +
                               "[cyan]cool[/] (heaters 0, fans on)  " +
                               "[green]balanced[/] (mixed)  " +
                               "[grey]off[/] (everything off)");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey dim]Press any key to return to the dashboard...[/]");
    }

    /// <inheritdoc/>
    public void RenderLogs()
    {
        AnsiConsole.WriteLine();
        
        var logs = SerilogQueSink.GetLogs();

        var logGrid = new Grid().AddColumn(new GridColumn());

        if (logs.Length == 0)
        {
            logGrid.AddRow(new Markup("[grey dim]No log entries captured yet.[/]"));
        }
        else
        {
            foreach (var entry in logs)
                logGrid.AddRow(new Markup(entry));
        }

        var panel = new Panel(logGrid)
            .Header($"[bold]Activity Logs[/] [grey]({logs.Length} entries)[/]")
            .Border(BoxBorder.Rounded)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey dim]Press any key to return to the dashboard...[/]");
    }

/// <summary>
    /// Renders the top header containing the application title and refresh timestamp.
    /// </summary>
    /// <param name="refreshedAt">Timestamp of the most recent model refresh.</param>
    private static void RenderHeader(DateTimeOffset refreshedAt)
    {
        var headerGrid = new Grid()
            .AddColumn(new GridColumn())
            .AddColumn(new GridColumn().NoWrap().RightAligned());

        headerGrid.AddRow(
            new Markup($"[grey]Refreshed: {refreshedAt:HH:mm:ss}[/]"));

        var panel = new Panel(headerGrid)
            .Border(BoxBorder.Rounded)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Renders sensor, heater, and fan panels in a three-column grid.
    /// </summary>
    /// <param name="model">Dashboard model used to build individual device panels.</param>
    private static void RenderDeviceGrid(DashboardModel model)
    {
        var sensorPanel = BuildSensorPanel(model);
        var heaterPanel = BuildHeaterPanel(model);
        var fanPanel = BuildFanPanel(model);

        var grid = new Grid()
            .AddColumn(new GridColumn())
            .AddColumn(new GridColumn())
            .AddColumn(new GridColumn());

        grid.AddRow(sensorPanel, heaterPanel, fanPanel);
        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Builds the sensor table panel, showing temperature and a derived thermal status per device ID.
    /// Missing sensors are displayed as error rows.
    /// </summary>
    /// <param name="model">Dashboard model containing sensor data and expected device count.</param>
    /// <returns>A configured <see cref="Panel"/> for sensor information.</returns>
    private static Panel BuildSensorPanel(DashboardModel model)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .Expand()
            .AddColumn(new TableColumn("[grey]#[/]").Width(3).Centered())
            .AddColumn(new TableColumn("[grey]Temperature[/]"))
            .AddColumn(new TableColumn("[grey]Status[/]").Width(6).Centered());

        for (int id = 1; id <= model.ExpectedDeviceCount; id++)
        {
            var sensor = model.Sensors.FirstOrDefault(s => s.Id == id);

            if (sensor is null)
            {
                table.AddRow(id.ToString(), "[red]N/A[/]", "[red]ERR[/]");
                continue;
            }

            var (color, label) = TemperatureCategory(sensor.Temperature.Value);
            table.AddRow(
                $"[white]{id}[/]",
                $"[{color}]{Markup.Escape(sensor.Temperature.Round().ToString())}[/]",
                $"[{color}]{label}[/]");
        }

        return new Panel(table)
            .Header("[bold cyan] Sensors [/]")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    /// <summary>
    /// Builds the heater table panel, showing per-device level and a visual intensity bar.
    /// Missing heaters are displayed as unavailable rows.
    /// </summary>
    /// <param name="model">Dashboard model containing heater data and expected device count.</param>
    /// <returns>A configured <see cref="Panel"/> for heater information.</returns>
    private static Panel BuildHeaterPanel(DashboardModel model)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .Expand()
            .AddColumn(new TableColumn("[grey]#[/]").Width(3).Centered())
            .AddColumn(new TableColumn("[grey]Level[/]").Width(5).Centered())
            .AddColumn(new TableColumn("[grey]Intensity[/]"));

        for (int id = 1; id <= model.ExpectedDeviceCount; id++)
        {
            var heater = model.Heaters.FirstOrDefault(h => h.Id == id);

            if (heater is null)
            {
                table.AddRow(id.ToString(), "[red]N/A[/]", "[red]---[/]");
                continue;
            }

            var color = HeaterLevelColor(heater.Level);
            var bar = BuildHeaterBar(heater.Level);
            table.AddRow(
                $"[white]{id}[/]",
                $"[{color}]{heater.Level}[/]",
                $"[{color}]{bar}[/]");
        }

        return new Panel(table)
            .Header("[bold yellow] Heaters [/]")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    /// <summary>
    /// Builds the fan table panel, showing per-device ON/OFF state.
    /// Missing fans are displayed as unavailable rows.
    /// </summary>
    /// <param name="model">Dashboard model containing fan data and expected device count.</param>
    /// <returns>A configured <see cref="Panel"/> for fan information.</returns>
    private static Panel BuildFanPanel(DashboardModel model)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .Expand()
            .AddColumn(new TableColumn("[grey]#[/]").Width(3).Centered())
            .AddColumn(new TableColumn("[grey]State[/]").Centered());

        for (int id = 1; id <= model.ExpectedDeviceCount; id++)
        {
            var fan = model.Fans.FirstOrDefault(f => f.Id == id);

            if (fan is null)
            {
                table.AddRow(id.ToString(), "[red]N/A[/]");
                continue;
            }

            var (label, color) = fan.Status
                ? ("  ON  ", "green bold")
                : ("  OFF  ", "grey");

            table.AddRow($"[white]{id}[/]", $"[{color}]{label}[/]");
        }

        return new Panel(table)
            .Header("[bold green] Fans [/]")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    /// <summary>
    /// Renders a feedback bar when a message is available.
    /// Success and error messages are visually distinguished by icon and color.
    /// </summary>
    /// <param name="message">Feedback text to display; if <see langword="null"/>, nothing is rendered.</param>
    /// <param name="isError"><see langword="true"/> for error styling; otherwise success styling.</param>
    private static void RenderFeedbackBar(string? message, bool isError)
    {
        if (message is null) return;

        var icon = isError ? "[red]✗[/]" : "[green]✓[/]";
        var content = $"{icon} {Markup.Escape(message)}";

        var panel = new Panel(new Markup(content))
            .Border(BoxBorder.Rounded)
            .Expand();

        AnsiConsole.Write(panel);
    }

    /// <summary>
    /// Renders a compact footer listing available keyboard shortcuts.
    /// </summary>
    private static void RenderShortcutBar()
    {
        AnsiConsole.MarkupLine(
            "[grey][[Ctrl+F]][/] Fan  " +
            "[grey][[Ctrl+H]][/] Heater  " +
            "[grey][[Ctrl+A]][/] All  " +
            "[grey][[Ctrl+R]][/] Reset  " +
            "[grey][[Ctrl+L]][/] Logs  " +
            "[grey][[r]][/] Refresh  " +
            "[grey][[?]][/] Help  " +
            "[grey][[q]][/] Quit");
    }

    /// <summary>
    /// Maps a temperature value to a Spectre color name and short status label.
    /// </summary>
    /// <param name="celsius">Temperature in degrees Celsius.</param>
    /// <returns>
    /// A tuple where <c>color</c> is a Spectre markup color token and <c>label</c> is a concise status string.
    /// </returns>
    private static (string color, string label) TemperatureCategory(double celsius) => celsius switch
    {
        < 10 => ("blue", "COLD"),
        < 18 => ("cyan", "COOL"),
        < 28 => ("green", "NORM"),
        < 36 => ("yellow", "WARM"),
        _ => ("red bold", "HOT!")
    };

    /// <summary>
    /// Returns the Spectre color name appropriate for a heater level.
    /// </summary>
    /// <param name="level">Heater level in the expected range 0-5.</param>
    /// <returns>A Spectre markup color token describing heater intensity.</returns>
    private static string HeaterLevelColor(int level) => level switch
    {
        0 => "grey",
        1 or 2 => "yellow",
        3 or 4 => "orange1",
        _ => "red bold"
    };

    /// <summary>
    /// Builds a fixed-width heater intensity bar and textual percentage/off/max indicator.
    /// </summary>
    /// <param name="level">Heater level in the expected range 0-5.</param>
    /// <returns>Formatted bar text for terminal display.</returns>
    private static string BuildHeaterBar(int level)
    {
        // TODO: Research Spectre.Console's 'Progress Displays'
        var filled = new string('█', level);
        var empty = new string('░', MaxHeaterLevel - level);
        var pct = level == 0 ? "OFF" : level == MaxHeaterLevel ? "MAX" : $"{level * 20}%";
        return $"{filled}{empty} {pct}";
    }
}
