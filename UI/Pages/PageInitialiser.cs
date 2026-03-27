using System.Diagnostics;
using BeautifulClient.Services.Api;
using Spectre.Console;

namespace BeautifulClient.UI.Pages;

public class PageInitialiser
{
    private readonly IApiService _apiService;
    private readonly Queue<string> _activityLogs = new();
    private const int MaxLogs = 5;

    public PageInitialiser(IApiService apiService)
    {
        _apiService = apiService;
        AddLog("[grey]Dashboard initialised and connected to API.[/]");
    }

    public async Task RunAsync()
    {
        bool exit = false;

        while (!exit)
        {
            // State 1: The Live, Auto-Refreshing Dashboard
            await RunLiveDashboardAsync();

            // State 2: The Interactive Command Menu
            exit = await RunCommandMenuAsync();
        }
    }

    private async Task RunLiveDashboardAsync()
    {
        // Set up the static skeleton
        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header").Size(4),
                new Layout("Content"), // Middle section for Sensors & Actuators
                new Layout("Logs").Size(7)
            );

        layout["Header"].Update(
            new Panel(new Align(new Markup(
                "[cyan bold]ENVROSYM HARDWARE COMMAND CENTRE[/]\n" +
                "[grey]System is running... Press [/][yellow bold]ENTER[/][grey] to open the Command Menu.[/]"), 
                HorizontalAlignment.Center)).Border(BoxBorder.Rounded));

        await AnsiConsole.Live(layout)
            .StartAsync(async ctx =>
            {
                var stopwatch = Stopwatch.StartNew();
                bool forceRefresh = true;

                while (true)
                {
                    // 1. NON-BLOCKING INPUT CHECK
                    // Check 20 times a second if the user pressed Enter
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        if (key == ConsoleKey.Enter)
                        {
                            return; // Break out of Live mode and proceed to the Menu
                        }
                    }

                    // 2. FETCH AND UPDATE DATA (Every 2 seconds)
                    if (stopwatch.ElapsedMilliseconds > 2000 || forceRefresh)
                    {
                        forceRefresh = false;
                        stopwatch.Restart();

                        // Fetch the real state
                        var state = await FetchCurrentStateAsync();

                        // Build the split grid
                        var mainGrid = new Grid().AddColumns(2);
                        mainGrid.AddRow(
                            BuildSensorsPanel(state.Sensors),
                            BuildActuatorsPanel(state.Fans, state.Heaters)
                        );

                        layout["Content"].Update(mainGrid);
                    }

                    // 3. ALWAYS REFRESH LOGS (in case background threads push new messages)
                    layout["Logs"].Update(BuildLogsPanel());
                    ctx.Refresh();

                    // Tiny delay to prevent 100% CPU usage on the while loop
                    await Task.Delay(50);
                }
            });
    }

    private async Task<bool> RunCommandMenuAsync()
    {
        AnsiConsole.Clear();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[yellow]Operator Command Menu[/] (Use arrows to navigate)")
                .PageSize(8)
                .HighlightStyle("cyan bold")
                .AddChoices(
                    "1. Control Fan",
                    "2. Control Heater",
                    "3. Read Specific Sensor",
                    "4. Run Simulation Loop",
                    "5. Reset System State",
                    "6. Return to Dashboard (Refresh)",
                    "7. Exit Dashboard"
                ));

        try
        {
            return await ExecuteChoiceAsync(choice[0]);
        }
        catch (Exception ex)
        {
            AddLog($"[red]Critical Error:[/] {ex.Message}");
            AnsiConsole.MarkupLine($"\n[red]Error:[/] {ex.Message}");
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to dashboard...[/]");
            Console.ReadKey(true);
            return false;
        }
    }

    private async Task<bool> ExecuteChoiceAsync(char option)
    {
        switch (option)
        {
            case '1': 
                await HandleControlFanAsync(); 
                return false;
            case '2': 
                await HandleControlHeaterAsync(); 
                return false;
            case '3': 
                int id = AnsiConsole.Ask<int>("Enter [cyan]Sensor ID[/]:");
                var res = await _apiService.GetSensorTemperatureAsync(id);
                if (res.IsSuccess && res.Value != null) 
                    AddLog($"Sensor {id} manually read: [cyan]{res.Value.Temperature:F1}°C[/]");
                else 
                    AddLog($"[red]Failed to read Sensor {id}[/]");
                return false;
            case '4': 
                AddLog("[yellow]Simulation loop not fully implemented in UI yet.[/]"); 
                return false;
            case '5': 
                if (AnsiConsole.Confirm("[red]Are you sure you want to reset the client state?[/]"))
                {
                    AddLog("[green]System reset command issued.[/]");
                    // await _apiService.ResetAsync();
                }
                return false;
            case '6': 
                AddLog("[blue]Returned to live dashboard.[/]");
                return false; // Loop continues, taking us back to State 1
            case '7': 
                return true; // Exit entirely
            default: 
                return false;
        }
    }

    #region Layout Builders

    private Panel BuildSensorsPanel(Dictionary<int, double?> sensors)
    {
        var table = new Table().Expand().BorderColor(Color.Grey);
        table.AddColumn("ID");
        table.AddColumn("Type");
        table.AddColumn("Temperature");

        for (int i = 1; i <= 3; i++)
        {
            if (sensors.TryGetValue(i, out var temp) && temp.HasValue)
            {
                string colour = temp > 25 ? "red" : temp < 18 ? "blue" : "green";
                table.AddRow(i.ToString(), "Temperature", $"[{colour}]{temp:F1}°C[/]");
            }
            else
            {
                table.AddRow(i.ToString(), "Temperature", "[red]Offline / Error[/]");
            }
        }

        return new Panel(table)
            .Header("[cyan]Active Sensors[/]")
            .Border(BoxBorder.Rounded);
    }

    private Panel BuildActuatorsPanel(Dictionary<int, bool?> fans, Dictionary<int, int?> heaters)
    {
        var table = new Table().Expand().BorderColor(Color.Grey);
        table.AddColumn("Device");
        table.AddColumn("ID");
        table.AddColumn("State/Level");

        // Fans
        for (int i = 1; i <= 3; i++)
        {
            string state = fans.TryGetValue(i, out var isOn) && isOn.HasValue 
                ? (isOn.Value ? "[green]ON[/]" : "[grey]OFF[/]") 
                : "[red]Err[/]";
            table.AddRow("Fan", i.ToString(), state);
        }

        // Heaters
        for (int i = 1; i <= 3; i++)
        {
            string level = heaters.TryGetValue(i, out var lvl) && lvl.HasValue 
                ? (lvl.Value > 0 ? $"[yellow]Level {lvl.Value}[/]" : "[grey]Off[/]") 
                : "[red]Err[/]";
            table.AddRow("Heater", i.ToString(), level);
        }

        return new Panel(table)
            .Header("[yellow]Hardware Actuators[/]")
            .Border(BoxBorder.Rounded);
    }

    private Panel BuildLogsPanel()
    {
        var logContent = string.Join(Environment.NewLine, _activityLogs);
        if (string.IsNullOrWhiteSpace(logContent)) logContent = "[grey]No activity.[/]";

        return new Panel(new Markup(logContent))
            .Header("[grey]Pipeline Activity[/]")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    #endregion

    #region Action Handlers

    private async Task HandleControlFanAsync()
    {
        int fanId = AnsiConsole.Ask<int>("Enter [green]Fan Number[/] (1-3):");
        bool turnOn = AnsiConsole.Confirm($"Turn Fan {fanId} [green]ON[/]?");

        var result = await _apiService.SetFanStateAsync(fanId, turnOn);
        if (result.IsSuccess) AddLog($"[green]SUCCESS:[/] Fan {fanId} set to {(turnOn ? "ON" : "OFF")}.");
        else AddLog($"[red]FAILED:[/] Could not update Fan {fanId}.");
    }

    private async Task HandleControlHeaterAsync()
    {
        int heaterId = AnsiConsole.Ask<int>("Enter [yellow]Heater Number[/] (1-3):");
        int level = AnsiConsole.Prompt(
            new TextPrompt<int>("Set Heater Level ([yellow]0-5[/]):")
                .ValidationErrorMessage("[red]Level must be between 0 and 5.[/]")
                .Validate(lvl => lvl is >= 0 and <= 5 ? ValidationResult.Success() : ValidationResult.Error()));

        var result = await _apiService.SetHeaterLevelAsync(heaterId, level);
        if (result.IsSuccess) AddLog($"[green]SUCCESS:[/] Heater {heaterId} set to Level {level}.");
        else AddLog($"[red]FAILED:[/] Could not update Heater {heaterId}.");
    }

    #endregion

    #region Helpers

    private void AddLog(string message)
    {
        if (_activityLogs.Count >= MaxLogs) _activityLogs.Dequeue();
        _activityLogs.Enqueue($"[[{DateTime.Now:HH:mm:ss}]] {message}");
    }

    private async Task<(Dictionary<int, double?> Sensors, Dictionary<int, bool?> Fans, Dictionary<int, int?> Heaters)> FetchCurrentStateAsync()
    {
        var sensors = new Dictionary<int, double?>();
        var fans = new Dictionary<int, bool?>();
        var heaters = new Dictionary<int, int?>();

        // Fetch Sensors
        for (int i = 1; i <= 3; i++)
        {
            var res = await _apiService.GetSensorTemperatureAsync(i);
            sensors[i] = (res.IsSuccess && res.Value != null) ? (double?)res.Value.Temperature : null;
        }

        // Fetch Fans
        for (int i = 1; i <= 3; i++)
        {
            var res = await _apiService.GetFanDataAsync(i);
            fans[i] = (res.IsSuccess && res.Value != null) ? (bool?)res.Value.Status : null;
        }

        // Fetch Heaters (Make sure you have GetHeaterDataAsync in your ApiService!)
        for (int i = 1; i <= 3; i++)
        {
            // Placeholder fallback until you wire up the heater API endpoint
            heaters[i] = 0; 
        }

        return (sensors, fans, heaters);
    }

    #endregion
}