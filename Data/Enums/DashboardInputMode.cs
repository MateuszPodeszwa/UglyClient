using BeautifulClient.UI.Commands;

namespace BeautifulClient.Data.Enums;

/// <summary>
/// Selects the grammar used by <see cref="ICommandParser"/> when parsing raw user input.
/// </summary>
public enum DashboardInputMode : byte
{
    /// <summary>Full command syntax, e.g. <c>fan 1 on</c>, <c>preset warm</c>, <c>reset</c>.</summary>
    Command,

    /// <summary>Shorthand fan syntax, e.g. <c>1 on</c>, <c>all off</c>, <c>1on 2 off</c>.</summary>
    Fan,

    /// <summary>Shorthand heater syntax, e.g. <c>1 3</c>, <c>2:5</c>, <c>all 0</c>.</summary>
    Heater
}