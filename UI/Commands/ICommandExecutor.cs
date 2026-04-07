using BeautifulClient.Data.Records;

namespace BeautifulClient.UI.Commands;

/// <summary>
/// Executes a <see cref="ParsedCommand"/> against the hardware API and returns a human-readable result.
/// </summary>
/// <remarks>
/// All mutations are delegated to <c>IApiService</c>, keeping this class free of direct HTTP concerns.
/// The returned <see cref="CommandResult"/> is forwarded by the controller to the view as dashboard feedback.
/// </remarks>
public interface ICommandExecutor
{
    /// <summary>
    /// Executes the given <paramref name="command"/> and returns the outcome.
    /// </summary>
    /// <param name="command">The resolved user intent to act on.</param>
    /// <returns>
    /// A <see cref="CommandResult"/> describing whether the operation succeeded and a message
    /// suitable for the dashboard feedback bar.
    /// </returns>
    Task<CommandResult> ExecuteAsync(ParsedCommand command);
}
