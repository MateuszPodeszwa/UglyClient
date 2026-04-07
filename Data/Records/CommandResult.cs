using BeautifulClient.UI.Commands;

namespace BeautifulClient.Data.Records;

/// <summary>
/// The outcome of executing a <see cref="ParsedCommand"/> via <see cref="ICommandExecutor"/>.
/// </summary>
/// <param name="IsSuccess">Whether the command completed without errors.</param>
/// <param name="Message">A human-readable description of the result, suitable for display in the dashboard feedback bar.</param>
public sealed record CommandResult(bool IsSuccess, string Message);