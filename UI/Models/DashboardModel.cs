using BeautifulClient.Data.Objects;

namespace BeautifulClient.UI.Models;

/// <summary>
/// Aggregates the live state of all hardware devices for a single dashboard render cycle.
/// </summary>
/// <remarks>
/// Populated by <c>DashboardController</c> after a parallel fetch of every device.
/// Missing device entries (failed API calls) are omitted from the lists; the renderer
/// fills those slots with an error indicator by comparing against <see cref="ExpectedDeviceCount"/>.
/// </remarks>
public sealed record DashboardModel
{
    /// <summary>Gets the successfully fetched sensor readings, keyed by <c>Id</c>.</summary>
    public IReadOnlyList<SensorData> Sensors { get; init; } = [];

    /// <summary>Gets the successfully fetched heater states, keyed by <c>Id</c>.</summary>
    public IReadOnlyList<HeaterData> Heaters { get; init; } = [];

    /// <summary>Gets the successfully fetched fan states, keyed by <c>Id</c>.</summary>
    public IReadOnlyList<FanData> Fans { get; init; } = [];

    /// <summary>Gets the number of physical devices of each type expected in the system.</summary>
    public int ExpectedDeviceCount { get; init; } = 3;

    /// <summary>Gets the timestamp at which this snapshot was assembled.</summary>
    public DateTimeOffset RefreshedAt { get; init; } = DateTimeOffset.Now;

    /// <summary>Gets the human-readable result of the most recently executed command, or <see langword="null"/> when none has been run yet.</summary>
    public string? LastFeedback { get; init; } // The Payload

    /// <summary>Gets a value indicating whether <see cref="LastFeedback"/> represents a failure.</summary>
    public bool IsFeedbackError { get; init; }
}
