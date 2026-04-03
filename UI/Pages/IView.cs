namespace BeautifulClient.UI.Pages;

public interface IView<in TModel>
{
    Task<(Type? nextRoute, object? payload)> ReturnAsync(TModel model);
}