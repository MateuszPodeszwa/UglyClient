using System.Diagnostics.CodeAnalysis;
using BeautifulClient.Data;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;

[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
public class HardwareApiService(
    HttpClient httpClient,
    ApiResultPipeline apiResultPipeline
    ) : ApiActions(httpClient), IHardwareApiService
{
    // The return type MUST be an ApiResult so the caller can check for success/failure
    public async Task<ApiResult<SensorData>> GetSensorTemperatureAsync(int sensorId)
    {
        // Pass the raw, unexecuted method into the pipeline via a lambda.
        // The pipeline handles the 'await' internally.
        return await apiResultPipeline.ExecuteAsync(() => GetAsync<SensorData>(
            $"api/sensor/{sensorId}",
            json => new()
            {
                Temperature = json.GetProperty("Temperature").GetSingle(),
                RawJson = json.GetRawText() 
            }
        ));
    }

    [Obsolete("WIP",true)]
    public Task SetHeaterLevelAsync(int heaterId, int level)
    {
        throw new NotImplementedException();
    }
    [Obsolete("WIP", true)]
    public Task SetFanStateAsync(int fanId, bool isOn)
    {
        throw new NotImplementedException();
    }
}