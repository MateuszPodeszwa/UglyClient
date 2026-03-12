using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BeautifulClient.Data;
using BeautifulClient.Extensions;
using BeautifulClient.Models;
using BeautifulClient.Utilities.RequestResultUtility;
using Microsoft.Extensions.Logging;

namespace BeautifulClient.Services.Api;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
public class HardwareApiService(HttpClient httpClient, ILogger<HardwareApiService> logger) : IHardwareApiService
{
    public async Task<ApiRequest> GetAsync<T>(string requestUri, Func<JsonElement, T> createData) where T : IData
    {
        HttpResponseMessage? responseMessage = null;
    
        try
        {
            responseMessage = await httpClient.GetAsync(requestUri);
            responseMessage.EnsureSuccessStatusCode();

            var responseMessageContent = await responseMessage.Content.ReadAsStringAsync();
        
            // Parse the raw string into a generic JSON Document
            using JsonDocument doc = JsonDocument.Parse(responseMessageContent);
        
            // Pass the raw JSON element to factory function
            T data = createData(doc.RootElement);

            return ApiRequest.Create(RequestResult.Success(), data);
        }
        
        #region Catch Exceptions
        catch (HttpRequestException e) 
        {
            logger.LogCritical("Request failed: {HttpResponseMessage}", responseMessage);
            // Must return a failed ApiRequest so the method signature is satisfied
            return ApiRequest.Create(RequestResult.Failure("Reqyest Failed",null, e), null!); 
        }
        catch (Exception e) 
        {
            logger.LogError(e, "Request for {responseMessage} has encountered exception, {Message}", responseMessage, e.Message);
            return ApiRequest.Create(RequestResult.Failure("Encountered Exception", null, e), null!);
        }
        finally 
        {
            responseMessage?.Dispose();
        }
        #endregion
    }

    public async Task<double> GetSensorTemperatureAsync(int sensorId)
    {
        var request = await GetAsync<SensorData>(
            $"api/sensor/{sensorId}", 
            json => new() 
            { 
                Temperature = json.GetDouble(), // TODO Make it a struct, then use explicit operators or function to try/catch and convert to double
                RawJson = json.GetRawText() 
            }
        );
    
        return request.Result.IsSuccess ? ((SensorData)request.Data).Temperature ?? 0.0 : double.NaN;
    }

    public async Task SetHeaterLevelAsync(int heaterId, int level)
    {
        HttpResponseMessage responseMessage = null!;

        try
        {
            responseMessage = await httpClient.PostAsync($"api/heat/{heaterId}", 
                new StringContent(level.ToString(), System.Text.Encoding.UTF8, "application/json"));
            responseMessage.EnsureSuccessStatusCode();
        }
        
        #region Catch Exceptions
        catch (HttpRequestException e)
        {
            logger.LogCritical(e, "HTTP request failed for heater {HeaterId}. Status Code: {StatusCode}", heaterId, responseMessage?.StatusCode);
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error fetching temperature for sensor {HeaterId}.", heaterId);
        }
        finally 
        {
            // Manually dispose HttpResponseMessage to avoid socked exhaustion - only if the responseMessage is not already null.
            responseMessage?.Dispose();
        }
        #endregion
    }

    public Task SetFanStateAsync(int fanId, bool isOn)
    {
        throw new NotImplementedException();
    }
}