using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Tests.UI.TestDoubles;

internal sealed class FakeApiService : IApiService
{
    public Func<int, ApiResult<SensorData>> OnGetSensor { get; set; } =
        id => TestObjectFactory.Sensor(id, 20 + id);

    public Func<int, ApiResult<HeaterData>> OnGetHeater { get; set; } =
        id => TestObjectFactory.Heater(id, id % 6);

    public Func<int, ApiResult<FanData>> OnGetFan { get; set; } =
        id => TestObjectFactory.Fan(id, id % 2 == 0);

    public Func<int, int, ApiResult> OnSetHeater { get; set; } =
        (_, _) => ApiResult.Success();

    public Func<int, bool, ApiResult> OnSetFan { get; set; } =
        (_, _) => ApiResult.Success();

    public Func<ApiResult> OnReset { get; set; } = () => ApiResult.Success();

    public List<int> GetSensorCalls { get; } = [];
    public List<int> GetHeaterCalls { get; } = [];
    public List<int> GetFanCalls { get; } = [];
    public List<(int HeaterId, int Level)> SetHeaterCalls { get; } = [];
    public List<(int FanId, bool IsOn)> SetFanCalls { get; } = [];
    public int ResetCalls { get; private set; }

    public Task<ApiResult<SensorData>> GetSensorTemperatureAsync(int sensorId)
    {
        GetSensorCalls.Add(sensorId);
        return Task.FromResult(OnGetSensor(sensorId));
    }

    public Task<ApiResult> SetHeaterLevelAsync(int heaterId, int level)
    {
        SetHeaterCalls.Add((heaterId, level));
        return Task.FromResult(OnSetHeater(heaterId, level));
    }

    public Task<ApiResult<HeaterData>> GetHeaterDataAsync(int heaterId)
    {
        GetHeaterCalls.Add(heaterId);
        return Task.FromResult(OnGetHeater(heaterId));
    }

    public Task<ApiResult> SetFanStateAsync(int fanId, bool isOn)
    {
        SetFanCalls.Add((fanId, isOn));
        return Task.FromResult(OnSetFan(fanId, isOn));
    }

    public Task<ApiResult<FanData>> GetFanDataAsync(int fanId)
    {
        GetFanCalls.Add(fanId);
        return Task.FromResult(OnGetFan(fanId));
    }

    public Task<ApiResult> ResetAsync()
    {
        ResetCalls++;
        return Task.FromResult(OnReset());
    }
}
