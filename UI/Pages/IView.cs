using BeautifulClient.UI.Controllers;

namespace BeautifulClient.UI.Pages;

public interface IView<in TModel>
{
    Task<NavigationResult> ReturnAsync(TModel model);
}