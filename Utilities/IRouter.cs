using BeautifulClient.Data.Records;
using BeautifulClient.UI.Controllers;

namespace BeautifulClient.Utilities;

/// <summary>
/// Defines a route-capable controller that can execute UI flow logic and return
/// the next navigation decision.
/// </summary>
public interface IRouter
{
    /// <summary>
    /// Executes the current route action using an optional payload and returns
    /// the resulting navigation instruction.
    /// </summary>
    /// <param name="payload">
    /// Optional input object passed from the previous route. Implementations are
    /// responsible for validating and interpreting the payload type.
    /// </param>
    /// <returns>
    /// A task that resolves to a <see cref="NavigationResult"/> describing the next route
    /// and optional payload to pass forward.
    /// </returns>
    Task<NavigationResult> ExecuteAsync(object? payload = null);
}