using BeautifulClient.Data.Objects;
using BeautifulClient.Data.Records;
using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeautifulClient.Tests.UI.Controllers;

public class HomePageControllerTests
{
    [Fact]
    public async Task ExecuteAsync_BuildsExpectedModel_WhenApiReturnsSensor()
    {
        var api = new FakeApiService
        {
            OnGetSensor = id => TestObjectFactory.Sensor(id, 22.5)
        };
        var view = new CapturingUserDashboardView();
        var subject = new HomePageController(api, view, NullLogger<HomePageController>.Instance);

        NavigationResult result = await subject.ExecuteAsync(payload: 2);

        Assert.NotNull(view.Model);
        Assert.Equal(1, view.Model.UserId);
        Assert.Equal(2, view.Model.SensorId);
        Assert.Equal("SystemAdmin", view.Model.Username);
        Assert.Equal("Active", view.Model.Status);
        Assert.Equal(2, view.Model.Payload);
        Assert.Equal(typeof(HomePageController), result.NextRoute);
    }

    [Fact]
    public async Task ExecuteAsync_UsesDefaultSensorId_WhenPayloadIsNull()
    {
        var api = new FakeApiService();
        var view = new CapturingUserDashboardView();
        var subject = new HomePageController(api, view, NullLogger<HomePageController>.Instance);

        await subject.ExecuteAsync(payload: null);

        Assert.Single(api.GetSensorCalls);
        Assert.Equal(0, api.GetSensorCalls[0]);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsDefaultModel_WhenApiFails()
    {
        var api = new FakeApiService
        {
            OnGetSensor = _ => ApiResult<SensorData>.Failure(Error.NetworkFailure)
        };
        var view = new CapturingUserDashboardView();
        var subject = new HomePageController(api, view, NullLogger<HomePageController>.Instance);

        await subject.ExecuteAsync(payload: 1);

        Assert.NotNull(view.Model);
        Assert.Equal(0, view.Model.UserId);
        Assert.Equal(0, view.Model.SensorId);
        Assert.Equal(string.Empty, view.Model.Username);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPayloadTypeIsInvalid()
    {
        var subject = new HomePageController(
            new FakeApiService(),
            new CapturingUserDashboardView(),
            NullLogger<HomePageController>.Instance);

        await Assert.ThrowsAsync<InvalidCastException>(() => subject.ExecuteAsync(payload: "invalid"));
    }

    private sealed class CapturingUserDashboardView : IView<UserDashboardModel>
    {
        public UserDashboardModel? Model { get; private set; }

        public Task<NavigationResult> ReturnAsync(UserDashboardModel model)
        {
            Model = model;
            return Task.FromResult(new NavigationResult(typeof(HomePageController), model.Payload));
        }
    }
}
