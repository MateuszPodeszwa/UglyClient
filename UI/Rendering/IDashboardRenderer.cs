using BeautifulClient.UI.Models;

namespace BeautifulClient.UI.Rendering;

/// <summary>
/// Abstracts all Spectre.Console output operations for the live dashboard.
/// </summary>
/// <remarks>
/// Separating rendering from input handling follows the Single Responsibility Principle:
/// <c>DashboardPage</c> owns the interaction loop; <c>IDashboardRenderer</c> owns what is drawn.
/// </remarks>
public interface IDashboardRenderer
{
    /// <summary>
    /// Clears the terminal and renders the complete dashboard layout for the given model snapshot.
    /// </summary>
    /// <param name="model">The aggregated device state to display.</param>
    void RenderDashboard(DashboardModel model);

    /// <summary>
    /// Clears the terminal and renders the keyboard shortcut reference overlay.
    /// Caller is responsible for waiting on user input before returning to the dashboard.
    /// </summary>
    void RenderHelp();

    /// <summary>
    /// Clears the terminal and renders the most recent activity log entries captured by <c>SerilogQueSink</c>.
    /// Caller is responsible for waiting on user input before returning to the dashboard.
    /// </summary>
    void RenderLogs();
}
