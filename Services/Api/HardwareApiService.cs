using System.Diagnostics.CodeAnalysis;
using BeautifulClient.Data;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;
/// <summary>
/// Provides the concrete implementation for communicating with the domain-specific hardware API endpoints.
/// </summary>
/// <param name="httpClient">The HTTP client instance used for network requests, passed down to the base <see cref="ApiActions"/> class.</param>
/// <param name="apiResultPipeline">The central execution pipeline that intercepts, logs, and times the API calls.</param>
/// <remarks>
/// <para><b>Purpose:</b> To explicitly define and encapsulate hardware-related HTTP calls, ensuring the consuming client remains completely oblivious to the data source (whether from a local physical sensor or a remote API).</para>
/// <para><b>Strategy:</b> Promotes reusability and a thin-client architecture. By acting as an integration boundary, this class validates arguments, formats endpoint URIs, and executes the call via the pipeline. The UI strictly receives a safe, formatted Result.</para>
/// <para><b>Pattern:</b> Functions as a Facade and an Adapter. It provides a highly simplified interface for the client to consume, whilst adapting raw JSON responses into the application's internal, strongly-typed domain models.</para>
/// </remarks>
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
                // Adapter, ITemperature do not care whether it's int, double, float, string etc.
                // It gets the job done.
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