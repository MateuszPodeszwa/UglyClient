using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.Services.Api;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class HardwareApiService(HttpClient httpClient, ILogger<HardwareApiService> logger) : IHardwareApiService
{
    public async Task<string> GetAsync(string requestUri)
    {
        HttpResponseMessage responseMessage = null!;
        try
        {
            responseMessage = await httpClient.GetAsync(requestUri);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            logger.LogCritical("Request failed: {HttpResponseMessage}", responseMessage);
            return e.ToString();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error fetching");
        
            return string.Empty;
        }
    }

    public async Task<double> GetSensorTemperatureAsync(int sensorId)
    {
        HttpResponseMessage responseMessage = null!;

        try
        {
            responseMessage = await httpClient.GetAsync($"api/sensors/{sensorId}");
        
            // Throws an HttpRequestException if the status is 4xx or 5xx
            responseMessage.EnsureSuccessStatusCode();

            var tempString = await responseMessage.Content.ReadAsStringAsync();

            // Use TryParse: It handles nulls, bad formats, and overflows safely!
            if (double.TryParse(tempString, out var temperature))
            {
                return temperature;
            }
            else
            {
                // The API returned a 200 OK, but the text wasn't a valid number
                logger.LogWarning("Sensor {SensorId} returned invalid data format: '{TempString}'", sensorId, tempString);
                return double.NaN; 
            }
        }
        catch (HttpRequestException e)
        {
            // Network failed, or the API returned a 404/500
            logger.LogCritical(e, "HTTP request failed for sensor {SensorId}. Status Code: {StatusCode}", 
                sensorId, responseMessage?.StatusCode);
            
            return double.NaN; 
        }
        catch (Exception e)
        {
            // A catch-all for anything truly unexpected (e.g., TaskCanceledException if it times out)
            logger.LogError(e, "Unexpected error fetching temperature for sensor {SensorId}.", sensorId);
        
            return double.NaN;
        }
    }

    public Task SetHeaterLevelAsync(int heaterId, int level)
    {
        throw new NotImplementedException();
    }

    public Task SetFanStateAsync(int fanId, bool isOn)
    {
        throw new NotImplementedException();
    }
}