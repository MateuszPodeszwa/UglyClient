namespace BeautifulClient.UI.Controllers;

public interface IRouter
{
    Task<Type?> ExecuteAsync(object? payload = null);
    object? Payload { get; set; }
}