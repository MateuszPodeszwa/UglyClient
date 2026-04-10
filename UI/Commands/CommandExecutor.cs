using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.UI.Commands;

/// <summary>
/// Implements <see cref="ICommandExecutor"/> by dispatching each <see cref="DashboardCommandType"/>
/// to the appropriate <see cref="IApiService"/> operation(s).
/// </summary>
/// <remarks>
/// Bulk operations (<see cref="DashboardCommandType.SetAllFans"/>,
/// <see cref="DashboardCommandType.SetAllHeaters"/>, <see cref="DashboardCommandType.ApplyPreset"/>)
/// run all underlying API calls in parallel via <c>Task.WhenAll</c>.
/// </remarks>
public sealed class CommandExecutor(IApiService apiService) : ICommandExecutor
{
    private const int DeviceCount = 3; // TODO: Move it to appsettings.json

    /// <inheritdoc/>
    public Task<CommandResult> ExecuteAsync(ParsedCommand command) => command.Type switch
    {
        DashboardCommandType.SetFan       => SetFanAsync(command),
        DashboardCommandType.SetAllFans   => SetAllFansAsync(command),
        DashboardCommandType.SetHeater    => SetHeaterAsync(command),
        DashboardCommandType.SetAllHeaters => SetAllHeatersAsync(command),
        DashboardCommandType.Reset        => ResetAsync(),
        DashboardCommandType.ApplyPreset  => ApplyPresetAsync(command),
        _ => Task.FromResult(new CommandResult(false, $"Unrecognised command: '{command.RawInput}'. Press [?] for help."))
    };

    // Fan

    private async Task<CommandResult> SetFanAsync(ParsedCommand command)
    {
        // Bulk shorthand: "1on 2off 3on" stores a list of (id, state) pairs in Value
        if (command.Value is IReadOnlyList<(int id, bool state)> bulk)
        {
            var results = await Task.WhenAll(
                bulk.Select(op => apiService.SetFanStateAsync(op.id, op.state)));

            int failures = results.Count(r => r.IsFailure);
            return failures == 0
                ? new CommandResult(true,  $"Updated {bulk.Count} fan(s).")
                : new CommandResult(false, $"{failures}/{results.Length} fan operation(s) failed.");
        }

        if (command.DeviceId is null || command.Value is not bool isOn)
            return new CommandResult(false, "Invalid fan command — expected: fan <id> on|off");

        var result = await apiService.SetFanStateAsync(command.DeviceId.Value, isOn);
        return ToCommandResult(result, $"Fan {command.DeviceId} turned {(isOn ? "ON" : "OFF")}.");
    }

    private async Task<CommandResult> SetAllFansAsync(ParsedCommand command)
    {
        if (command.Value is not bool isOn)
            return new CommandResult(false, "Invalid fan state — expected: on or off");

        var results = await Task.WhenAll(
            Enumerable.Range(1, DeviceCount).Select(i => apiService.SetFanStateAsync(i, isOn)));

        int failures = results.Count(r => r.IsFailure);
        return failures == 0
            ? new CommandResult(true,  $"All fans turned {(isOn ? "ON" : "OFF")}.")
            : new CommandResult(false, $"{failures}/{DeviceCount} fans failed to update.");
    }

    // Heater

    private async Task<CommandResult> SetHeaterAsync(ParsedCommand command)
    {
        if (command.DeviceId is null || command.Value is not int level)
            return new CommandResult(false, "Invalid heater command — expected: heater <id> <0-5>");

        var result = await apiService.SetHeaterLevelAsync(command.DeviceId.Value, level);
        return ToCommandResult(result, $"Heater {command.DeviceId} set to level {level}.");
    }

    private async Task<CommandResult> SetAllHeatersAsync(ParsedCommand command)
    {
        if (command.Value is not int level)
            return new CommandResult(false, "Invalid heater level — expected: 0–5");

        var results = await Task.WhenAll(
            Enumerable.Range(1, DeviceCount).Select(i => apiService.SetHeaterLevelAsync(i, level)));

        int failures = results.Count(r => r.IsFailure);
        return failures == 0
            ? new CommandResult(true,  $"All heaters set to level {level}.")
            : new CommandResult(false, $"{failures}/{DeviceCount} heaters failed to update.");
    }

    private async Task<CommandResult> ResetAsync()
    {
        var result = await apiService.ResetAsync();
        return ToCommandResult(result, "Simulation reset successfully.");
    }

    // Preset

    private async Task<CommandResult> ApplyPresetAsync(ParsedCommand command)
    {
        if (command.Value is not string presetName)
            return new CommandResult(false, "Invalid preset — expected: preset <name>");

        if (!TemperaturePreset.TryGet(presetName, out var preset) || preset is null)
            return new CommandResult(false,
                $"Unknown preset '{presetName}'. Available: {TemperaturePreset.AvailableNames}");

        var heaterTasks = preset.HeaterLevels
            .Select((lvl, i) => apiService.SetHeaterLevelAsync(i + 1, lvl));

        var fanTasks = preset.FanStates
            .Select((state, i) => apiService.SetFanStateAsync(i + 1, state));

        var allResults = await Task.WhenAll(heaterTasks.Concat(fanTasks));
        int failures = allResults.Count(r => r.IsFailure);

        return failures == 0
            ? new CommandResult(true,  $"Preset '{preset.Name}' applied.")
            : new CommandResult(false, $"Preset '{preset.Name}' partially applied ({failures} failure(s)).");
    }

    // Helpers

    private static CommandResult ToCommandResult(ApiResult result, string successMessage)
        => result.IsSuccess
            ? new CommandResult(true,  successMessage)
            : new CommandResult(false, $"Operation failed: {result.Error.Message}");
}
