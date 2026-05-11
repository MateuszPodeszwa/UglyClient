using BeautifulClient.Data.Records;
using BeautifulClient.UI.Commands;

namespace BeautifulClient.Tests.UI.Commands;

public class TemperaturePresetTests
{
    [Theory]
    [InlineData("warm")]
    [InlineData("cool")]
    [InlineData("balanced")]
    [InlineData("off")]
    [InlineData("WARM")]
    public void TryGet_ReturnsPreset_ForKnownNames(string name)
    {
        bool found = TemperaturePreset.TryGet(name, out TemperaturePreset? preset);

        Assert.True(found);
        Assert.NotNull(preset);
        Assert.Equal(3, preset.HeaterLevels.Count);
        Assert.Equal(3, preset.FanStates.Count);
    }

    [Fact]
    public void TryGet_ReturnsFalse_ForUnknownName()
    {
        bool found = TemperaturePreset.TryGet("polar", out TemperaturePreset? preset);

        Assert.False(found);
        Assert.Null(preset);
    }

    [Fact]
    public void AvailableNames_ContainsAllRegisteredPresets()
    {
        string names = TemperaturePreset.AvailableNames;

        Assert.Contains("warm", names, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("cool", names, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("balanced", names, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("off", names, StringComparison.OrdinalIgnoreCase);
    }
}
