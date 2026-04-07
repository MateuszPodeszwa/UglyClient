using BeautifulClient.Data.Enums;
using BeautifulClient.UI.Commands;

namespace BeautifulClient.Data.Records;

/// <summary>
/// An immutable value produced by <see cref="ICommandParser"/> that describes the user's intent.
/// </summary>
/// <param name="Type">The resolved action type.</param>
/// <param name="DeviceId">
/// The targeted device identifier (1-based), or <see langword="null"/> when targeting all devices.
/// </param>
/// <param name="Value">
/// The command argument — <see cref="bool"/> for fan state, <see cref="int"/> for heater level,
/// <see cref="string"/> for preset name, or a bulk list of <c>(int id, bool state)</c> tuples for
/// multi-fan shorthand input.
/// </param>
/// <param name="RawInput">The original, unmodified string entered by the user.</param>
public sealed record ParsedCommand(
    DashboardCommandType Type,
    int? DeviceId = null,
    object? Value = null,
    string? RawInput = null);