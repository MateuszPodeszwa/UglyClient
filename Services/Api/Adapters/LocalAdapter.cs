using System.Text.Json;
using BeautifulClient.Data;
using BeautifulClient.Services.Api.Actions;
using BeautifulClient.Services.Hardware;
using BeautifulClient.Utilities.ErrorHandler;

// ReSharper disable All

namespace BeautifulClient.Services.Api.Adapters;

public class LocalAdapter(
    HttpClient httpClient,
    ApiResultPipeline apiResultPipeline
) : ApiActions(httpClient), IApiService
{
    public async Task<ApiResult<SensorData>> GetSensorTemperatureAsync(int sensorId)
    {
        var data = await HardwarePlugService.GetTemperatureFloatAsync(sensorId);

        if (data is null)
        {
            return (ApiResult<SensorData>) Error.LocalApiFail;
        }
        
        return await apiResultPipeline.ExecuteAsync((() => GetAsync<SensorData>(
            data.ToString(),
            json => new()
            {
                Id =  sensorId,
                Temperature = json.GetDouble(),
                RawJson =  json.GetRawText()
            }
            )));
    }

    public Task<ApiResult> SetHeaterLevelAsync(int heaterId, int level)
    {
        return Task.FromResult((ApiResult)Error.LocalApiFail);
    }

    public Task<ApiResult> SetFanStateAsync(int fanId, bool isOn)
    {
        return Task.FromResult((ApiResult)Error.LocalApiFail);
    }

    // I decided to implement string in the base GetAsync as every type and class has .ToString method.
    public override Task<ApiResult<T>> GetAsync<T>(string? data, Func<JsonElement, T> createData)
    {
        try
        {
            if (string.IsNullOrEmpty(data))
            {
                return Task.FromResult((ApiResult<T>)Error.CustomHttpError(000, "Received null or empty data string"));
            }
            
            using JsonDocument doc = JsonDocument.Parse(data);
            T parsedData = createData(doc.RootElement);
            
            return Task.FromResult<ApiResult<T>>(parsedData);
        }
        catch (JsonException)
        {
            return Task.FromResult((ApiResult<T>)Error.InvalidJson);
        }
        catch (Exception exception)
        {
            return Task.FromResult((ApiResult<T>)Error.CustomHttpError(exception.HResult, exception.Message));
        }
    }
}