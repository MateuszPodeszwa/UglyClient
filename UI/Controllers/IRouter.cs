namespace BeautifulClient.UI.Controllers;

public interface IRouter
{
    Task<Type?> ExecuteAsync();
}