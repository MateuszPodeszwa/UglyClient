using BeautifulClient.UI.Models;

namespace BeautifulClient.Tests.UI.Models;

public class UiModelsTests
{
    [Fact]
    public void DashboardModel_HasExpectedDefaults()
    {
        var model = new DashboardModel();

        Assert.Empty(model.Sensors);
        Assert.Empty(model.Heaters);
        Assert.Empty(model.Fans);
        Assert.Equal(3, model.ExpectedDeviceCount);
        Assert.Null(model.LastFeedback);
        Assert.False(model.IsFeedbackError);
    }

    [Fact]
    public void UserDashboardModel_InitializesStringPropertiesToEmpty()
    {
        var model = new UserDashboardModel();

        Assert.Equal(string.Empty, model.Username);
        Assert.Equal(string.Empty, model.Status);
        Assert.Null(model.Payload);
    }

    [Fact]
    public void FanSensorPageModel_AllowsSimplePropertyRoundTrip()
    {
        var model = new FanSensorPageModel
        {
            SensorId = 3,
            IsOnline = true
        };

        Assert.Equal(3, model.SensorId);
        Assert.True(model.IsOnline);
    }
}
