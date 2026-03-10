namespace BeautifulClient.Services.Api;

/// <summary>
/// Defines a contract between API and application. Lists all actions that API can perform.
/// </summary>
public interface IApiService
{
    Task<string> GetAsync();
}