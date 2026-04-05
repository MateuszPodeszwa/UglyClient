namespace BeautifulClient.UI.Controllers;

public interface IRouter
{
    Task<NavigationResult> ExecuteAsync(object? payload = null);
}