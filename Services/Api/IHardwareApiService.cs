using BeautifulClient.Extensions;
using BeautifulClient.Utilities.RequestResultUtility;

namespace BeautifulClient.Services.Api;

/// <summary>
/// Defines a contract between API and application. Lists all actions that API can perform.
/// </summary>
public interface IHardwareApiService
{
    /// <summary>
    /// Asynchronously retrieves the current temperature reading for a specific sensor.
    /// </summary>
    /// <remarks>
    /// This method queries the sensor API endpoint. If the HTTP request fails, the response content 
    /// cannot be parsed into a valid number, or any other unexpected error occurs, the error is logged 
    /// and the method gracefully returns <see cref="double.NaN"/>.
    /// </remarks>
    /// <param name="sensorId">The unique identifier of the sensor to query.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains the temperature 
    /// value, or <see cref="double.NaN"/> if the retrieval or parsing fails.
    /// </returns>
    Task<double> GetSensorTemperatureAsync(int sensorId);
    Task SetHeaterLevelAsync(int heaterId, int level);
    Task SetFanStateAsync(int fanId, bool isOn);
}