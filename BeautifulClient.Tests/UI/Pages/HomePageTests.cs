using BeautifulClient.Data.Records;
using BeautifulClient.Data.Structs.Temperature;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using Spectre.Console;

namespace BeautifulClient.Tests.UI.Pages;

public class HomePageTests
{
    [Fact]
    public async Task ReturnAsync_DisplaysModelAndReturnsSelectedSensorRoute()
    {
        var input = new StubHomePageInput(4);
        var subject = new HomePage(input);
        var model = new UserDashboardModel
        {
            UserId = 10,
            SensorId = 2,
            Temperature = new Celcius(21.2),
            Username = "Operator",
            Status = "Active",
            Payload = 99
        };

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(model);
        string output = AnsiConsole.ExportText();

        Assert.Contains("Welcome, Operator", output);
        Assert.Contains("Temperature: [21.2 °C]", output);
        Assert.Contains("Sensor ID: [2]", output);
        Assert.Contains("Status: Active", output);
        Assert.Equal(1, input.Calls);
        Assert.Equal(typeof(HomePageController), result.NextRoute);
        Assert.Equal(4, result.Payload);
    }

    private sealed class StubHomePageInput(int sensorId) : IHomePageInput
    {
        public int Calls { get; private set; }

        public int PromptSensorId()
        {
            Calls++;
            return sensorId;
        }
    }
}
