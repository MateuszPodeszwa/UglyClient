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
    public object? Payload { get; set; }

    private async Task<ApiResult<SensorData>> TestGetUserId()
    {
        return await Api.GetSensorTemperatureAsync(2);
    }

    public async Task<Type?> ExecuteAsync(object? payload = null)
    {
        logger.LogWarning("Executing HomePageController.ExecuteAsync");
        logger.LogWarning($"payload: {payload}");
        var result = await TestGetUserId();
        
        var model = new UserDashboardModel
        {
            UserId = 1,
            SensorId = result.Value.Id,
            Temperature = result.Value.Temperature,
            Username = "SystemAdmin",
            Status = "Active",
            Payload = payload
        };
        
        var view = await View.ReturnAsync(model);
        Payload = view.payload;
        
        return view.nextRoute;
    }
};