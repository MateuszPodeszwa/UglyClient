namespace BeautifulClient.UI.Pages;

public interface IView<in TModel>
{
    Task<Type?> ReturnAsync(TModel model);
}