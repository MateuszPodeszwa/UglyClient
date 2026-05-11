using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;

namespace BeautifulClient.UI.Commands;

/// <summary>
/// Implements <see cref="ICommandParser"/> by routing the raw input to a grammar-specific
/// sub-parser based on the active <see cref="DashboardInputMode"/>.
/// </summary>
/// <remarks>
/// <para><b>Command mode grammar (full syntax):</b></para>
/// <list type="bullet">
///   <item><description><c>fan &lt;id&gt; on|off</c></description></item>
///   <item><description><c>fan all on|off</c></description></item>
///   <item><description><c>heater &lt;id&gt; &lt;0-5&gt;</c></description></item>
///   <item><description><c>heater all &lt;0-5&gt;</c></description></item>
///   <item><description><c>preset &lt;name&gt;</c></description></item>
///   <item><description><c>reset</c></description></item>
///   <item><description><c>r</c> / <c>refresh</c></description></item>
///   <item><description><c>q</c> / <c>quit</c> / <c>exit</c></description></item>
/// </list>
/// <para><b>Fan mode shorthand:</b> <c>1 on</c>, <c>2 off</c>, <c>all on</c>, <c>1on 2off 3on</c></para>
/// <para><b>Heater mode shorthand:</b> <c>1 3</c>, <c>2:5</c>, <c>all 0</c></para>
/// </remarks>
public sealed class CommandParser : ICommandParser
{
    /// <inheritdoc/>
    public ParsedCommand Parse(string input, DashboardInputMode mode = DashboardInputMode.Command)
    {
        var trimmed = input.Trim();

        // Unrecognised Input
        if (string.IsNullOrWhiteSpace(trimmed))
            return new ParsedCommand(DashboardCommandType.Refresh, RawInput: input);

        return mode switch
        {
            DashboardInputMode.Fan    => ParseFanShorthand(trimmed),
            DashboardInputMode.Heater => ParseHeaterShorthand(trimmed),
            _                         => ParseCommandSyntax(trimmed)
        };
    }

    // Full command syntax

    private static ParsedCommand ParseCommandSyntax(string input)
    {
        var parts = SplitTokens(input);

        return parts[0].ToLowerInvariant() switch
        {
            "q" or "quit" or "exit" => new ParsedCommand(DashboardCommandType.Quit, RawInput: input),
            "r" or "refresh"        => new ParsedCommand(DashboardCommandType.Refresh, RawInput: input),
            "reset"                 => new ParsedCommand(DashboardCommandType.Reset, RawInput: input),
            "fan"    when parts.Length >= 3 => ParseFanTokens(parts, input),
            "heater" when parts.Length >= 3 => ParseHeaterTokens(parts, input),
            "preset" when parts.Length >= 2 => new ParsedCommand(DashboardCommandType.ApplyPreset, Value: parts[1], RawInput: input),
            _                               => new ParsedCommand(DashboardCommandType.Unknown, RawInput: input)
        };
    }

    private static ParsedCommand ParseFanTokens(string[] parts, string rawInput)
    {
        if (parts[1].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return ParseOnOff(parts[2]) is { } state
                ? new ParsedCommand(DashboardCommandType.SetAllFans, Value: state, RawInput: rawInput)
                : new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);
        }

        if (!int.TryParse(parts[1], out int fanId))
            return new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);

        return ParseOnOff(parts[2]) is { } fanState
            ? new ParsedCommand(DashboardCommandType.SetFan, DeviceId: fanId, Value: fanState, RawInput: rawInput)
            : new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);
    }

    private static ParsedCommand ParseHeaterTokens(string[] parts, string rawInput)
    {
        if (parts[1].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHeaterLevel(parts[2], out int allLevel)
                ? new ParsedCommand(DashboardCommandType.SetAllHeaters, Value: allLevel, RawInput: rawInput)
                : new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);
        }

        if (!int.TryParse(parts[1], out int heaterId))
            return new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);

        return TryParseHeaterLevel(parts[2], out int level)
            ? new ParsedCommand(DashboardCommandType.SetHeater, DeviceId: heaterId, Value: level, RawInput: rawInput)
            : new ParsedCommand(DashboardCommandType.Unknown, RawInput: rawInput);
    }

    // Fan mode shorthand

    private static ParsedCommand ParseFanShorthand(string input)
    {
        var tokens = SplitTokens(input);

        // "all on" / "all off"
        if (tokens.Length == 2 && tokens[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return ParseOnOff(tokens[1]) is { } state
                ? new ParsedCommand(DashboardCommandType.SetAllFans, Value: state, RawInput: input)
                : new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);
        }

        // Single token inline: "1on" / "2off"
        if (tokens.Length == 1)
            return ParseInlineFanToken(tokens[0], input)
                   ?? new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);

        // "1 on" / "2 off"
        if (tokens.Length == 2 && int.TryParse(tokens[0], out int singleId))
        {
            return ParseOnOff(tokens[1]) is { } singleState
                ? new ParsedCommand(DashboardCommandType.SetFan, DeviceId: singleId, Value: singleState, RawInput: input)
                : new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);
        }

        // Multi-shorthand "1on 2off 3on"
        var bulk = new List<(int id, bool state)>();
        foreach (var token in tokens)
        {
            var parsed = ParseInlineFanToken(token, rawInput: null);
            if (parsed?.Value is bool s && parsed.DeviceId.HasValue)
                bulk.Add((parsed.DeviceId.Value, s));
        }

        return bulk.Count > 0
            ? new ParsedCommand(DashboardCommandType.SetFan, Value: (IReadOnlyList<(int, bool)>)bulk.AsReadOnly(), RawInput: input)
            : new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);
    }

    /// <summary>Parses a single inline token such as <c>1on</c> or <c>2off</c>.</summary>
    private static ParsedCommand? ParseInlineFanToken(string token, string? rawInput)
    {
        if (token.EndsWith("on", StringComparison.OrdinalIgnoreCase))
        {
            var idStr = token[..^2];
            if (int.TryParse(idStr, out int id))
                return new ParsedCommand(DashboardCommandType.SetFan, DeviceId: id, Value: true, RawInput: rawInput);
        }

        if (token.EndsWith("off", StringComparison.OrdinalIgnoreCase))
        {
            var idStr = token[..^3];
            if (int.TryParse(idStr, out int id))
                return new ParsedCommand(DashboardCommandType.SetFan, DeviceId: id, Value: false, RawInput: rawInput);
        }

        return null;
    }

    // Heater mode shorthand

    private static ParsedCommand ParseHeaterShorthand(string input)
    {
        // Colon separator so "2:5" becomes "2 5"
        var parts = SplitTokens(input.Replace(':', ' '));

        if (parts.Length < 2)
            return new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);

        if (parts[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHeaterLevel(parts[1], out int allLevel)
                ? new ParsedCommand(DashboardCommandType.SetAllHeaters, Value: allLevel, RawInput: input)
                : new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);
        }

        if (!int.TryParse(parts[0], out int heaterId))
            return new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);

        return TryParseHeaterLevel(parts[1], out int level)
            ? new ParsedCommand(DashboardCommandType.SetHeater, DeviceId: heaterId, Value: level, RawInput: input)
            : new ParsedCommand(DashboardCommandType.Unknown, RawInput: input);
    }

    // Helpers

    private static string[] SplitTokens(string input)
        => input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool? ParseOnOff(string s) => s.ToLowerInvariant() switch
    {
        "on"  or "true"  or "1" => true,
        "off" or "false" or "0" => false,
        _                       => null
    };

    private static bool TryParseHeaterLevel(string s, out int level)
        => int.TryParse(s, out level) && level is >= 0 and <= 5;
}
