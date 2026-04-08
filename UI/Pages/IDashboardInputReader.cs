namespace BeautifulClient.UI.Pages;

/// <summary>
/// Reads keyboard and line input for the dashboard interaction loop.
/// </summary>
public interface IDashboardInputReader
{
    /// <summary>
    /// Reads a single key from input.
    /// </summary>
    /// <param name="intercept">When <see langword="true"/>, the key is not echoed to the terminal.</param>
    /// <returns>The key that was read.</returns>
    ConsoleKeyInfo ReadKey(bool intercept);

    /// <summary>
    /// Reads a full input line.
    /// </summary>
    /// <returns>The entered line, or <see langword="null"/> at end-of-stream.</returns>
    string? ReadLine();
}