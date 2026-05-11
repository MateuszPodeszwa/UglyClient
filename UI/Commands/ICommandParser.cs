using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;

namespace BeautifulClient.UI.Commands;

/// <summary>
/// Parses raw user input strings into strongly-typed <see cref="ParsedCommand"/> instances.
/// </summary>
/// <remarks>
/// The active <see cref="DashboardInputMode"/> adjusts the grammar used so that shorthand
/// syntax (e.g. <c>1on 2off</c> in fan mode, <c>2:3</c> in heater mode) is recognised
/// without requiring the device-type prefix.
/// </remarks>
public interface ICommandParser
{
    /// <summary>
    /// Parses the given <paramref name="input"/> according to the specified <paramref name="mode"/>.
    /// </summary>
    /// <param name="input">The raw string entered by the user.</param>
    /// <param name="mode">
    /// The grammar context. Defaults to <see cref="DashboardInputMode.Command"/> (full syntax).
    /// </param>
    /// <returns>
    /// A <see cref="ParsedCommand"/> whose <c>Type</c> is <see cref="DashboardCommandType.Unknown"/>
    /// when the input cannot be matched to any known pattern.
    /// </returns>
    ParsedCommand Parse(string input, DashboardInputMode mode = DashboardInputMode.Command);
}
