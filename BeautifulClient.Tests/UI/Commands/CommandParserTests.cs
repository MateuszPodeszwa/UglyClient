using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.UI.Commands;

namespace BeautifulClient.Tests.UI.Commands;

public class CommandParserTests
{
    private readonly CommandParser parser = new();

    [Fact]
    public void Parse_ReturnsRefresh_WhenInputIsWhitespace()
    {
        ParsedCommand result = parser.Parse("   ");

        Assert.Equal(DashboardCommandType.Refresh, result.Type);
        Assert.Equal("   ", result.RawInput);
    }

    [Theory]
    [InlineData("q")]
    [InlineData("quit")]
    [InlineData("exit")]
    public void Parse_ReturnsQuit_ForQuitAliases(string input)
    {
        ParsedCommand result = parser.Parse(input);

        Assert.Equal(DashboardCommandType.Quit, result.Type);
        Assert.Equal(input, result.RawInput);
    }

    [Theory]
    [InlineData("r")]
    [InlineData("refresh")]
    public void Parse_ReturnsRefresh_ForRefreshAliases(string input)
    {
        ParsedCommand result = parser.Parse(input);

        Assert.Equal(DashboardCommandType.Refresh, result.Type);
        Assert.Equal(input, result.RawInput);
    }

    [Fact]
    public void Parse_ReturnsReset_ForResetKeyword()
    {
        ParsedCommand result = parser.Parse("reset");

        Assert.Equal(DashboardCommandType.Reset, result.Type);
    }

    [Fact]
    public void Parse_ReturnsSingleFanCommand_ForFanIdAndState()
    {
        ParsedCommand result = parser.Parse("fan 2 on");

        Assert.Equal(DashboardCommandType.SetFan, result.Type);
        Assert.Equal(2, result.DeviceId);
        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void Parse_ReturnsSetAllFans_ForFanAllCommand()
    {
        ParsedCommand result = parser.Parse("fan all off");

        Assert.Equal(DashboardCommandType.SetAllFans, result.Type);
        Assert.Equal(false, result.Value);
    }

    [Fact]
    public void Parse_ReturnsSingleHeaterCommand_ForHeaterIdAndLevel()
    {
        ParsedCommand result = parser.Parse("heater 3 5");

        Assert.Equal(DashboardCommandType.SetHeater, result.Type);
        Assert.Equal(3, result.DeviceId);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void Parse_ReturnsSetAllHeaters_ForHeaterAllCommand()
    {
        ParsedCommand result = parser.Parse("heater all 1");

        Assert.Equal(DashboardCommandType.SetAllHeaters, result.Type);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Parse_ReturnsApplyPreset_ForPresetCommand()
    {
        ParsedCommand result = parser.Parse("preset warm");

        Assert.Equal(DashboardCommandType.ApplyPreset, result.Type);
        Assert.Equal("warm", result.Value);
    }

    [Fact]
    public void Parse_ReturnsUnknown_ForInvalidCommandSyntax()
    {
        ParsedCommand result = parser.Parse("heater 3 9");

        Assert.Equal(DashboardCommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_FanMode_ReturnsSetAllFans_ForAllOn()
    {
        ParsedCommand result = parser.Parse("all on", DashboardInputMode.Fan);

        Assert.Equal(DashboardCommandType.SetAllFans, result.Type);
        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void Parse_FanMode_ReturnsSetFan_ForSplitTokenSyntax()
    {
        ParsedCommand result = parser.Parse("2 off", DashboardInputMode.Fan);

        Assert.Equal(DashboardCommandType.SetFan, result.Type);
        Assert.Equal(2, result.DeviceId);
        Assert.Equal(false, result.Value);
    }

    [Fact]
    public void Parse_FanMode_ReturnsSetFanBulk_ForInlineMultiSyntax()
    {
        ParsedCommand result = parser.Parse("1on 2off 3on", DashboardInputMode.Fan);

        Assert.Equal(DashboardCommandType.SetFan, result.Type);
        Assert.NotNull(result.Value);

        var bulk = Assert.IsAssignableFrom<IReadOnlyList<(int id, bool state)>>(result.Value);
        Assert.Equal([(1, true), (2, false), (3, true)], bulk);
    }

    [Fact]
    public void Parse_FanMode_ReturnsUnknown_ForInvalidInput()
    {
        ParsedCommand result = parser.Parse("all maybe", DashboardInputMode.Fan);

        Assert.Equal(DashboardCommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_HeaterMode_ReturnsSetHeater_ForColonSyntax()
    {
        ParsedCommand result = parser.Parse("2:4", DashboardInputMode.Heater);

        Assert.Equal(DashboardCommandType.SetHeater, result.Type);
        Assert.Equal(2, result.DeviceId);
        Assert.Equal(4, result.Value);
    }

    [Fact]
    public void Parse_HeaterMode_ReturnsSetAllHeaters_ForAllSyntax()
    {
        ParsedCommand result = parser.Parse("all 0", DashboardInputMode.Heater);

        Assert.Equal(DashboardCommandType.SetAllHeaters, result.Type);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Parse_HeaterMode_ReturnsUnknown_ForOutOfRangeLevel()
    {
        ParsedCommand result = parser.Parse("1 8", DashboardInputMode.Heater);

        Assert.Equal(DashboardCommandType.Unknown, result.Type);
    }
}
