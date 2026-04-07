using BeautifulClient.Data.Objects;
using BeautifulClient.Data.Structs.Temperature;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Tests.UI.TestDoubles;

internal static class TestObjectFactory
{
    public static SensorData Sensor(int id, double temperatureCelsius)
    {
        return new SensorData(PassThroughObjectSetterPipeline.Instance)
        {
            Id = id,
            Temperature = new Celcius(temperatureCelsius),
            RawJson = $$"""{"id":{{id}},"temperature":{{temperatureCelsius}}}""",
            SaveAction = _ => Task.FromResult(ApiResult.Success())
        };
    }

    public static HeaterData Heater(int id, int level)
    {
        return new HeaterData(PassThroughObjectSetterPipeline.Instance)
        {
            Id = id,
            Level = level,
            RawJson = $$"""{"id":{{id}},"level":{{level}}}""",
            SaveAction = _ => Task.FromResult(ApiResult.Success())
        };
    }

    public static FanData Fan(int id, bool isOn)
    {
        return new FanData(PassThroughObjectSetterPipeline.Instance)
        {
            Id = id,
            Status = isOn,
            RawJson = $$"""{"id":{{id}},"status":{{isOn.ToString().ToLowerInvariant()}}}""",
            SaveAction = _ => Task.FromResult(ApiResult.Success())
        };
    }
}
