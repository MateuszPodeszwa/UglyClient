namespace BeautifulClient.UI.Commands;

/// <summary>
/// Discriminates every action the user can request from the dashboard.
/// </summary>
public enum DashboardCommandType : byte
{
    /// <summary>Re-fetch all device data without executing any mutation.</summary>
    Refresh,

    /// <summary>Terminate the application loop.</summary>
    Quit,

    /// <summary>Toggle a single fan on or off.</summary>
    SetFan,

    /// <summary>Toggle every fan simultaneously.</summary>
    SetAllFans,

    /// <summary>Set the power level (0–5) of a single heater.</summary>
    SetHeater,

    /// <summary>Set the power level (0–5) of every heater simultaneously.</summary>
    SetAllHeaters,

    /// <summary>POST a reset request to the simulation endpoint.</summary>
    Reset,

    /// <summary>Apply a named temperature preset across all devices.</summary>
    ApplyPreset,

    /// <summary>Input that could not be matched to any known command.</summary>
    Unknown
}