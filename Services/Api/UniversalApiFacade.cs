using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api.Adapters;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;

// TODO: Consider to implement handler in here, chain of responsibility pattern

public class UniversalApiFacade(
    LocalAdapter localService,
    RemoteAdapter remoteService
) : IApiService
{
    public async Task<ApiResult<SensorData>> GetSensorTemperatureAsync(int sensorId)
    {
        var localResult = await localService.GetSensorTemperatureAsync(sensorId);
        
        if (localResult.IsSuccess)
        {
            return localResult;
        }
        
        return await remoteService.GetSensorTemperatureAsync(sensorId);
    }

    public async Task<ApiResult> SetHeaterLevelAsync(int heaterId, int level)
    {
        var localResult = await localService.SetHeaterLevelAsync(heaterId, level);
        
        if (localResult.IsSuccess)
        {
            return localResult;
        }

        return await remoteService.SetHeaterLevelAsync(heaterId, level);
    }

    public async Task<ApiResult<HeaterData>> GetHeaterDataAsync(int heaterId)
    {
        var localResult = await localService.GetHeaterDataAsync(heaterId);
        
        if (localResult.IsSuccess)
        {
            return localResult;
        }
        
        return await remoteService.GetHeaterDataAsync(heaterId);
    }

    public async Task<ApiResult> SetFanStateAsync(int fanId, bool isOn)
    {
        var localResult = await localService.SetFanStateAsync(fanId, isOn);
        
        if (localResult.IsSuccess)
        {
            return localResult;
        }
        
        return await remoteService.SetFanStateAsync(fanId, isOn);
    }

    public async Task<ApiResult<FanData>> GetFanDataAsync(int fanId)
    {
        var localResult = await localService.GetFanDataAsync(fanId);
        
        if (localResult.IsSuccess)
        {
            return localResult;
        }
        
        return await remoteService.GetFanDataAsync(fanId);
    }
}