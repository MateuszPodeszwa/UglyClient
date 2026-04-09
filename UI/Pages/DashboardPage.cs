using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.UI.Commands;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Rendering;
using Spectre.Console;

namespace BeautifulClient.UI.Pages;

/// <summary>
/// The primary live-dashboard view. Renders the current device snapshot and handles all
/// keyboard shortcuts, entering named input modes or performing instant actions.
/// </summary>
/// <remarks>
/// <para><b>Interaction model:</b></para>
/// <list type="bullet">
///   <item><description>The renderer draws the full screen; this class owns only the interaction loop.</description></item>
///   <item><description>
///     Input is read with <c>Console.ReadKey(intercept: true)</c> so keystrokes are not echoed.
///     Ctrl combinations are detected via <see cref="ConsoleModifiers"/>.
///   </description></item>
///   <item><description>
///     Modal prompts (fan / heater / command modes) print an inline header and read a single line,
///     keeping the dashboard visible above.
///   </description></item>
///   <item><description>
///     Overlay screens (help, logs) clear the terminal, wait for any key, then signal a
///     <see cref="DashboardCommandType.Refresh"/> so the controller re-renders the dashboard.
///   </description></item>
/// </list>
/// </remarks>
public sealed class DashboardPage : IView<DashboardModel>
{
    private readonly IDashboardRenderer renderer;
    private readonly ICommandParser commandParser;
    private readonly IDashboardInputReader inputReader;

    /// <summary>
    /// Creates a dashboard page using the default console input reader.
    /// </summary>
    public DashboardPage(IDashboardRenderer renderer, ICommandParser commandParser)
        : this(renderer, commandParser, new ConsoleDashboardInputReader())
    {
    }

    /// <summary>
    /// Creates a dashboard page with explicit dependencies, including a custom input reader.
    /// </summary>
    /// <param name="renderer">Renderer responsible for drawing dashboard and overlays.</param>
    /// <param name="commandParser">Parser used for fan/heater/command input modes.</param>
    /// <param name="inputReader">Input reader used for keyboard and modal text input.</param>
    public DashboardPage(
        IDashboardRenderer renderer,
        ICommandParser commandParser,
        IDashboardInputReader inputReader)
    {
        this.renderer = renderer;
        this.commandParser = commandParser;
        this.inputReader = inputReader;
    }

    /// <inheritdoc/>
    public Task<NavigationResult> ReturnAsync(DashboardModel model)
    {
        renderer.RenderDashboard(model);

        AnsiConsole.Markup("[grey]>[/] ");
        var key     = inputReader.ReadKey(intercept: true);
        var command = DispatchKey(key);

        return Task.FromResult(new NavigationResult(typeof(DashboardController), command));
    }

    // Key dispatcher
    
    /// <summary>
    /// Maps a <see cref="ConsoleKeyInfo"/> to the appropriate action — either returning
    /// a <see cref="ParsedCommand"/> immediately or entering a modal prompt.
    /// </summary>
    private ParsedCommand DispatchKey(ConsoleKeyInfo key)
    {
        bool isCtrl = key.Modifiers.HasFlag(ConsoleModifiers.Control);

        if (isCtrl)
        {
            return key.Key switch
            {
                ConsoleKey.F => EnterFanMode(),
                ConsoleKey.H => EnterHeaterMode(),
                ConsoleKey.A => EnterCommandMode(),
                ConsoleKey.R => ConfirmReset(),
                ConsoleKey.L => ShowLogsOverlay(),
                _            => Refresh()
            };
        }

        return key.KeyChar switch
        {
            'q' or 'Q' => Quit(),
            'r' or 'R' => Refresh(),
            '?'        => ShowHelpOverlay(),
            _          => Refresh()
        };
    }

    // Modal input modes

    /// <summary>
    /// Displays the fan-mode prompt and parses the entered shorthand.
    /// </summary>
    private ParsedCommand EnterFanMode()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[cyan bold]FAN MODE[/]  " +
            "[grey]Syntax: <id> on|off  |  all on|off  |  1on 2off 3on[/]");
        AnsiConsole.Markup("[cyan]fan>[/] ");
        string lineInput = inputReader.ReadLine() ?? string.Empty;
        return commandParser.Parse(lineInput, DashboardInputMode.Fan);
    }

    /// <summary>
    /// Displays the heater-mode prompt and parses the entered shorthand.
    /// </summary>
    private ParsedCommand EnterHeaterMode()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[yellow bold]HEATER MODE[/]  " +
            "[grey]Syntax: <id> <0-5>  |  all <0-5>  |  <id>:<level>[/]");
        AnsiConsole.Markup("[yellow]heater>[/] ");
        string lineInput = inputReader.ReadLine() ?? string.Empty;
        return commandParser.Parse(lineInput, DashboardInputMode.Heater);
    }

    /// <summary>
    /// Displays the all-devices command prompt and parses full command syntax.
    /// </summary>
    private ParsedCommand EnterCommandMode()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "[magenta bold]ALL DEVICES[/]  " +
            "[grey]fan all on|off  |  heater all <0-5>  |  preset warm|cool|balanced|off[/]");
        AnsiConsole.Markup("[magenta]cmd>[/] ");
        string lineInput = inputReader.ReadLine() ?? string.Empty;
        return commandParser.Parse(lineInput, DashboardInputMode.Command);
    }

    /// <summary>
    /// Asks for y/n confirmation before issuing a reset command.
    /// </summary>
    private ParsedCommand ConfirmReset()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[red bold]RESET[/]  Reset the simulation? [grey](y/n)[/]  ");
        var confirm = inputReader.ReadKey(intercept: true);
        AnsiConsole.WriteLine();
        return confirm.KeyChar is 'y' or 'Y'
            ? new ParsedCommand(DashboardCommandType.Reset)
            : Refresh();
    }

    // Overlay screens

    /// <summary>
    /// Shows the help overlay and waits for any key before returning a refresh signal.
    /// </summary>
    private ParsedCommand ShowHelpOverlay()
    {
        renderer.RenderHelp();
        inputReader.ReadKey(intercept: true);
        return Refresh();
    }

    /// <summary>
    /// Shows the activity log overlay and waits for any key before returning a refresh signal.
    /// </summary>
    private ParsedCommand ShowLogsOverlay()
    {
        renderer.RenderLogs();
        inputReader.ReadKey(intercept: true);
        return Refresh();
    }

    // Helpers

    private static ParsedCommand Refresh()
        => new(DashboardCommandType.Refresh);

    private static ParsedCommand Quit()
        => new(DashboardCommandType.Quit);
}
