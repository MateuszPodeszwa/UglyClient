namespace BeautifulClient.Services.Api;

/// <summary>
/// Defines a contract between API and application. Lists all actions that API can perform.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Returns httpClient.GetAsync with requestUri
    /// </summary>
    /// <param name="requestUri"></param>
    /// <returns></returns>
    Task<string> GetAsync(string requestUri);
    Task<double> GetSensorTemperatureAsync(int sensorId);
    Task SetHeaterLevelAsync(int heaterId, int level);
    Task SetFanStateAsync(int fanId, bool isOn);
}