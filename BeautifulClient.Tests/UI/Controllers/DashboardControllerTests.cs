using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Objects;
using BeautifulClient.Data.Records;
using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.UI.Commands;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeautifulClient.Tests.UI.Controllers;

public class DashboardControllerTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsNullRoute_WhenQuitCommandIsPassed()
    {
        var subject = CreateSubject(
            out var api,
            out var view,
            out var executor);

        NavigationResult result = await subject.ExecuteAsync(new ParsedCommand(DashboardCommandType.Quit));

        Assert.Null(result.NextRoute);
        Assert.Empty(api.GetSensorCalls);
        Assert.Equal(0, executor.Calls);
        Assert.Null(view.Model);
    }

    [Fact]
    public async Task ExecuteAsync_RefreshesModelWithoutExecutingMutation_WhenCommandIsRefresh()
    {
        var subject = CreateSubject(
            out var api,
            out var view,
            out var executor);

        NavigationResult result = await subject.ExecuteAsync(new ParsedCommand(DashboardCommandType.Refresh));

        Assert.Equal(typeof(DashboardController), result.NextRoute);
        Assert.Equal(0, executor.Calls);
        Assert.Equal([1, 2, 3], api.GetSensorCalls);
        Assert.Equal([1, 2, 3], api.GetHeaterCalls);
        Assert.Equal([1, 2, 3], api.GetFanCalls);
        Assert.NotNull(view.Model);
        Assert.Equal(3, view.Model.Sensors.Count);
        Assert.Equal(3, view.Model.Heaters.Count);
        Assert.Equal(3, view.Model.Fans.Count);
        Assert.Null(view.Model.LastFeedback);
    }

    [Fact]
    public async Task ExecuteAsync_MapsUnknownCommandToErrorFeedback()
    {
        var subject = CreateSubject(
            out _,
            out var view,
            out var executor);

        await subject.ExecuteAsync(new ParsedCommand(DashboardCommandType.Unknown, RawInput: "wat"));

        Assert.Equal(0, executor.Calls);
        Assert.NotNull(view.Model);
        Assert.True(view.Model.IsFeedbackError);
        Assert.Contains("Unknown command", view.Model.LastFeedback);
    }

    [Fact]
    public async Task ExecuteAsync_AttachesExecutorFeedback_WhenMutationCommandRuns()
    {
        var subject = CreateSubject(
            out _,
            out var view,
            out var executor);
        executor.Result = new CommandResult(true, "Fan updated.");

        await subject.ExecuteAsync(new ParsedCommand(DashboardCommandType.SetFan, DeviceId: 1, Value: true));

        Assert.Equal(1, executor.Calls);
        Assert.NotNull(view.Model);
        Assert.Equal("Fan updated.", view.Model.LastFeedback);
        Assert.False(view.Model.IsFeedbackError);
    }

    [Fact]
    public async Task ExecuteAsync_OmitsFailedFetches_FromModelCollections()
    {
        var api = new FakeApiService
        {
            OnGetSensor = id => id == 2
                ? ApiResult<SensorData>.Failure(Error.NetworkFailure)
                : TestObjectFactory.Sensor(id, 20 + id),
            OnGetHeater = id => id == 1
                ? ApiResult<HeaterData>.Failure(Error.Timeout)
                : TestObjectFactory.Heater(id, id),
            OnGetFan = id => id == 3
                ? ApiResult<FanData>.Failure(Error.NotFound404)
                : TestObjectFactory.Fan(id, true)
        };
        var view = new CapturingDashboardView();
        var subject = new DashboardController(
            api,
            view,
            new StubCommandExecutor(),
            NullLogger<DashboardController>.Instance);

        await subject.ExecuteAsync(new ParsedCommand(DashboardCommandType.Refresh));

        Assert.NotNull(view.Model);
        Assert.Equal(2, view.Model.Sensors.Count);
        Assert.Equal(2, view.Model.Heaters.Count);
        Assert.Equal(2, view.Model.Fans.Count);
        Assert.Equal(3, view.Model.ExpectedDeviceCount);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPayloadTypeIsInvalid()
    {
        var subject = CreateSubject(out _, out _, out _);

        await Assert.ThrowsAsync<InvalidCastException>(() => subject.ExecuteAsync(payload: "bad-payload"));
    }

    private static DashboardController CreateSubject(
        out FakeApiService api,
        out CapturingDashboardView view,
        out StubCommandExecutor executor)
    {
        api = new FakeApiService();
        view = new CapturingDashboardView();
        executor = new StubCommandExecutor();

        return new DashboardController(
            api,
            view,
            executor,
            NullLogger<DashboardController>.Instance);
    }

    private sealed class CapturingDashboardView : IView<DashboardModel>
    {
        public DashboardModel? Model { get; private set; }

        public Task<NavigationResult> ReturnAsync(DashboardModel model)
        {
            Model = model;
            return Task.FromResult(new NavigationResult(typeof(DashboardController), model));
        }
    }

    private sealed class StubCommandExecutor : ICommandExecutor
    {
        public int Calls { get; private set; }
        public CommandResult Result { get; set; } = new(true, "ok");

        public Task<CommandResult> ExecuteAsync(ParsedCommand command)
        {
            Calls++;
            return Task.FromResult(Result);
        }
    }
}
