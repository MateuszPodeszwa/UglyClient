using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;

namespace BeautifulClient.UI.Pages;

public class FanSensorPage : IView<FanSensorPageModel>
{
    public Task<Type?> ReturnAsync(FanSensorPageModel model)
    {
        return Task.FromResult(typeof(HomePageController))!;
    }
}