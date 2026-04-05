using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities.Attributes;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.UI.Controllers;

[MenuRoute("HomeDashboard")]
public sealed class HomePageController(IApiService apiService, IView<UserDashboardModel> view, ILogger<HomePageController> logger) : Controller(apiService)
{
    private IView<UserDashboardModel> View { get; } = view;

    private async Task<ApiResult<SensorData>> TestGetUserId(int no)
    {
        return await Api.GetSensorTemperatureAsync(no);
    }

    public override async Task<NavigationResult> ExecuteAsync(object? payload = null)
    {
        logger.LogWarning("Executing HomePageController.ExecuteAsync");
        logger.LogWarning($"payload: {payload}");

        int sensorId = PayloadAs(payload, 0);
        var result = await TestGetUserId(sensorId);

        if (!result.IsSuccess) return await View.ReturnAsync(new UserDashboardModel());
        
        var model = new UserDashboardModel
        {
            UserId = 1,
            SensorId = result.Value.Id,
            Temperature = result.Value.Temperature,
            Username = "SystemAdmin",
            Status = "Active",
            Payload = sensorId
        };

        return await View.ReturnAsync(model);

    }
};