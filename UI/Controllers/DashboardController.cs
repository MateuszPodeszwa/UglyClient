using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.Services.Api;
using BeautifulClient.UI.Commands;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities;
using BeautifulClient.Utilities.Attributes;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.UI.Controllers;

/// <summary>
/// Orchestrates the live dashboard loop: executes any pending command, fetches fresh device data
/// in parallel, and delegates rendering to <see cref="IView{DashboardModel}"/>.
/// </summary>
/// <remarks>
/// <para><b>Flow:</b></para>
/// <list type="number">
///   <item><description>Inspect the incoming <paramref name="payload"/> for a <see cref="ParsedCommand"/>.</description></item>
///   <item><description>Exit the loop when the command type is <see cref="DashboardCommandType.Quit"/>.</description></item>
///   <item><description>Delegate mutations to <see cref="ICommandExecutor"/>, capturing the <see cref="CommandResult"/>.</description></item>
///   <item><description>Fetch all sensor, heater, and fan data in parallel.</description></item>
///   <item><description>Build a <see cref="DashboardModel"/> and call the view, which returns the next command as payload.</description></item>
/// </list>
/// </remarks>
[MenuRoute("Dashboard")] // WIP: To be implemented
public sealed class DashboardController(
    IApiService apiService,
    IView<DashboardModel> view,
    ICommandExecutor commandExecutor,
    ILogger<DashboardController> logger
) : Controller(apiService)
{
    private const int DeviceCount = 3; // Summed

    /// <inheritdoc/>
    public override async Task<NavigationResult> ExecuteAsync(object? payload = null)
    {
        var command = PayloadAs<ParsedCommand?>(payload, defaultValue: null);

        if (command?.Type is DashboardCommandType.Quit)
        {
            logger.LogInformation("Dashboard received Quit — exiting navigation loop.");
            return new NavigationResult(null);
        }

        CommandResult? lastResult = await TryExecuteCommandAsync(command);

        var model = await FetchDashboardModelAsync(lastResult);

        return await view.ReturnAsync(model);
    }

    // Command execution

    /// <summary>
    /// Executes <paramref name="command"/> via <see cref="ICommandExecutor"/> when it represents
    /// a mutation. Returns <see langword="null"/> for Refresh or null commands.
    /// </summary>
    private async Task<CommandResult?> TryExecuteCommandAsync(ParsedCommand? command)
    {
        if (command is null or { Type: DashboardCommandType.Refresh })
            return null;

        if (command.Type is DashboardCommandType.Unknown)
        {
            logger.LogWarning("Unknown command entered: '{Input}'", command.RawInput);
            return new CommandResult(false,
                $"Unknown command: '{command.RawInput}'. Press [?] for help.");
        }

        logger.LogInformation("Executing command: {Type}", command.Type);
        return await commandExecutor.ExecuteAsync(command);
    }

    /// <summary>
    /// Fetches all device data in parallel and assembles a <see cref="DashboardModel"/>.
    /// Failed API calls are silently omitted — the renderer shows an error indicator for missing IDs.
    /// </summary>
    private async Task<DashboardModel> FetchDashboardModelAsync(CommandResult? lastResult)
    {
        var sensorTasks = Enumerable.Range(1, DeviceCount)
            .Select(i => Api.GetSensorTemperatureAsync(i))
            .ToList();

        var heaterTasks = Enumerable.Range(1, DeviceCount)
            .Select(i => Api.GetHeaterDataAsync(i))
            .ToList();

        var fanTasks = Enumerable.Range(1, DeviceCount)
            .Select(i => Api.GetFanDataAsync(i))
            .ToList();

        await Task.WhenAll(
            Task.WhenAll(sensorTasks),
            Task.WhenAll(heaterTasks),
            Task.WhenAll(fanTasks));

        return new DashboardModel
        {
            Sensors = sensorTasks
                .Select(t => t.Result)
                .Where(r => r.IsSuccess)
                .Select(r => r.Value)
                .ToList(),

            Heaters = heaterTasks
                .Select(t => t.Result)
                .Where(r => r.IsSuccess)
                .Select(r => r.Value)
                .ToList(),

            Fans = fanTasks
                .Select(t => t.Result)
                .Where(r => r.IsSuccess)
                .Select(r => r.Value)
                .ToList(),

            ExpectedDeviceCount = DeviceCount,
            RefreshedAt         = DateTimeOffset.Now,
            LastFeedback        = lastResult?.Message,
            IsFeedbackError     = lastResult?.IsSuccess == false
        };
    }
}
