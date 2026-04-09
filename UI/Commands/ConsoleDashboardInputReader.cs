using BeautifulClient.UI.Pages;

namespace BeautifulClient.UI.Commands;

/// <summary>
/// Default <see cref="IDashboardInputReader"/> implementation backed by <see cref="Console"/>.
/// </summary>
public sealed class ConsoleDashboardInputReader : IDashboardInputReader
{
    /// <inheritdoc/>
    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

    /// <inheritdoc/>
    public string? ReadLine() => Console.ReadLine();
}