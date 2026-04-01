using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities.Attributes;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.UI.Controllers;

[MenuRoute("HomeDashboard")]
public sealed class HomePageController(IApiService apiService, IView<UserDashboardModel> view, ILogger<HomePageController> logger) : Controller(apiService), IRouter
{
    private IView<UserDashboardModel> View { get; } = view;

    private async Task<ApiResult<SensorData>> TestGetUserId()
    {
        return await Api.GetSensorTemperatureAsync(2);
    }

    public async Task<Type?> ExecuteAsync()
    {
        logger.LogWarning("Executing HomePageController.ExecuteAsync");
        var result = await TestGetUserId();
        
        var model = new UserDashboardModel
        {
            UserId = 1,
            SensorId = result.Value.Id,
            Temperature = result.Value.Temperature,
            Username = "SystemAdmin",
            Status = "Active"
        };
        
        return await View.ReturnAsync(model);
    }
};