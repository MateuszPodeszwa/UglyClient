using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.UI.Commands;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Tests.UI.Commands;

public class CommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsFailure_ForUnknownCommand()
    {
        var subject = CreateSubject();

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.Unknown, RawInput: "??"));

        Assert.False(result.IsSuccess);
        Assert.Contains("Unrecognised command", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_SetFanSingle_CallsApiAndReturnsSuccessMessage()
    {
        var api = new FakeApiService();
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetFan, DeviceId: 2, Value: true));

        Assert.True(result.IsSuccess);
        Assert.Contains("Fan 2 turned ON", result.Message);
        Assert.Equal([(2, true)], api.SetFanCalls);
    }

    [Fact]
    public async Task ExecuteAsync_SetFanSingle_ReturnsFailure_WhenApiFails()
    {
        var api = new FakeApiService
        {
            OnSetFan = (_, _) => ApiResult.Failure(Error.NetworkFailure)
        };
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetFan, DeviceId: 2, Value: false));

        Assert.False(result.IsSuccess);
        Assert.Contains("Operation failed", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_SetFanBulk_ReturnsPartialFailure_WhenAnyOperationFails()
    {
        var api = new FakeApiService
        {
            OnSetFan = (id, _) => id == 2 ? ApiResult.Failure(Error.NetworkFailure) : ApiResult.Success()
        };
        var subject = CreateSubject(api);
        IReadOnlyList<(int, bool)> bulk = [(1, true), (2, false), (3, true)];

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetFan, Value: bulk));

        Assert.False(result.IsSuccess);
        Assert.Contains("1/3 fan operation(s) failed", result.Message);
        Assert.Equal(3, api.SetFanCalls.Count);
    }

    [Fact]
    public async Task ExecuteAsync_SetAllFans_CallsApiForAllKnownDevices()
    {
        var api = new FakeApiService();
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetAllFans, Value: true));

        Assert.True(result.IsSuccess);
        Assert.Equal(3, api.SetFanCalls.Count);
        Assert.Equal([(1, true), (2, true), (3, true)], api.SetFanCalls);
    }

    [Fact]
    public async Task ExecuteAsync_SetHeaterSingle_CallsApiAndReturnsSuccess()
    {
        var api = new FakeApiService();
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetHeater, DeviceId: 1, Value: 4));

        Assert.True(result.IsSuccess);
        Assert.Equal([(1, 4)], api.SetHeaterCalls);
    }

    [Fact]
    public async Task ExecuteAsync_SetAllHeaters_ReturnsFailure_WhenAnySetFails()
    {
        var api = new FakeApiService
        {
            OnSetHeater = (id, _) => id == 3 ? ApiResult.Failure(Error.Timeout) : ApiResult.Success()
        };
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.SetAllHeaters, Value: 2));

        Assert.False(result.IsSuccess);
        Assert.Contains("1/3 heaters failed to update", result.Message);
        Assert.Equal(3, api.SetHeaterCalls.Count);
    }

    [Fact]
    public async Task ExecuteAsync_Reset_CallsApiAndReturnsSuccess()
    {
        var api = new FakeApiService();
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.Reset));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, api.ResetCalls);
    }

    [Fact]
    public async Task ExecuteAsync_ApplyPreset_ReturnsFailure_ForUnknownPreset()
    {
        var subject = CreateSubject();

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.ApplyPreset, Value: "arctic"));

        Assert.False(result.IsSuccess);
        Assert.Contains("Unknown preset", result.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ApplyPreset_AppliesHeatersAndFansForKnownPreset()
    {
        var api = new FakeApiService();
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.ApplyPreset, Value: "warm"));

        Assert.True(result.IsSuccess);
        Assert.Equal(3, api.SetHeaterCalls.Count);
        Assert.Equal(3, api.SetFanCalls.Count);
    }

    [Fact]
    public async Task ExecuteAsync_ApplyPreset_ReturnsPartialFailure_WhenAnyOperationFails()
    {
        var api = new FakeApiService
        {
            OnSetHeater = (id, _) => id == 1 ? ApiResult.Failure(Error.NetworkFailure) : ApiResult.Success()
        };
        var subject = CreateSubject(api);

        CommandResult result = await subject.ExecuteAsync(
            new ParsedCommand(DashboardCommandType.ApplyPreset, Value: "balanced"));

        Assert.False(result.IsSuccess);
        Assert.Contains("partially applied", result.Message);
    }

    private static CommandExecutor CreateSubject(FakeApiService? api = null)
        => new(api ?? new FakeApiService());
}
