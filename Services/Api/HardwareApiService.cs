using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.Services.Api;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class HardwareApiService(HttpClient httpClient, ILogger<HardwareApiService> logger) : IHardwareApiService
{
    public async Task<string> GetAsync(string requestUri)
    {
        HttpResponseMessage responseMessage = null!; try
        {
            responseMessage = await httpClient.GetAsync(requestUri);
            // Throws an HttpRequestException if the status is 4xx or 5xx
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStringAsync();
        }
        #region Catch Exceptions
        catch (HttpRequestException e) 
        {
            logger.LogCritical("Request failed: {HttpResponseMessage}", responseMessage);
            return e.ToString();
        }
        catch (Exception e) 
        {
            logger.LogError(e, "Request for {responseMessage} has encountered exception, {Message}", responseMessage, e.Message);
        
            return string.Empty;
        }
        finally 
        {
            // Manually dispose HttpResponseMessage to avoid socked exhaustion - only if the responseMessage is not already null.
            responseMessage?.Dispose();
        }
        #endregion
    }

    public async Task<double> GetSensorTemperatureAsync(int sensorId)
    {
        HttpResponseMessage responseMessage = null!; try
        {
            responseMessage = await httpClient.GetAsync($"api/sensor/{sensorId}");
            
            // Throws an HttpRequestException if the status is 4xx or 5xx
            responseMessage.EnsureSuccessStatusCode();
            
            var responseMessageContent = await responseMessage.Content.ReadAsStringAsync();

            if (double.TryParse(responseMessageContent, out var temperature))
            {
                logger.LogInformation("Sensor {SensorId} returned valid data temperature {Temperature}", sensorId, temperature);
                return temperature;
            }
            
            logger.LogWarning("Sensor {SensorId} returned invalid data format: '{responseMessageContent}'", sensorId, responseMessageContent);
            return double.NaN;
        }
        #region Catch Exceptions
        catch (HttpRequestException e)
        {
            logger.LogCritical(e, "HTTP request failed for sensor {SensorId}. Status Code: {StatusCode}", sensorId, responseMessage?.StatusCode);
            return double.NaN; 
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error fetching temperature for sensor {SensorId}.", sensorId);
            return double.NaN;
        }
        finally 
        {
            // Manually dispose HttpResponseMessage to avoid socked exhaustion - only if the responseMessage is not already null.
            responseMessage?.Dispose();
        }
        #endregion
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