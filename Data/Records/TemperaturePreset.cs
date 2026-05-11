namespace BeautifulClient.Data.Records;

/// <summary>
/// Defines a named configuration that sets all heater levels and fan states simultaneously.
/// </summary>
/// <remarks>
/// Presets are looked up by name (case-insensitive) via <see cref="TryGet"/>.
/// New presets should be added to <see cref="Catalogue"/> and exposed as static fields.
/// </remarks>
/// <param name="Name">The unique, lowercase identifier used in command input (e.g. <c>warm</c>).</param>
/// <param name="HeaterLevels">
/// Ordered list of heater levels (0–5) to apply. Index 0 maps to device ID 1.
/// </param>
/// <param name="FanStates">
/// Ordered list of fan states (<see langword="true"/> = on) to apply. Index 0 maps to device ID 1.
/// </param>
public sealed record TemperaturePreset(
    string Name,
    IReadOnlyList<int> HeaterLevels,
    IReadOnlyList<bool> FanStates)
{
    
    // ¡€#¢∞§¶•ªºœ∑´œ¡€#¢œ∑´´´®†¥¨^øπåß∂ƒ©˙∆˚¬…æΩ≈ç√∫~µ≤≥µ± - It is in the language of elves!
    /// <summary>Heaters at medium, fans off — suitable for warming the environment.</summary>
    private static readonly 
        TemperaturePreset Warm = new("warm", [3, 3, 3], [false, false, false]);

    /// <summary>Heaters off, fans fully on — maximum active cooling.</summary>
    private static readonly 
        TemperaturePreset Cool = new("cool", [0, 0, 0], [true, true, true]);

    /// <summary>Moderate heating with partial fan activity — steady-state comfort.</summary>
    private static readonly 
        TemperaturePreset Balanced = new("balanced", [2, 1, 2], [true, false, true]);

    /// <summary>All devices off — standby / idle state.</summary>
    private static readonly 
        TemperaturePreset Off = new("off", [0, 0, 0], [false, false, false]);
    
    /// <summary>
    /// Stores all predefined temperature presets keyed by preset name using case-insensitive lookup.
    /// </summary>
    private static readonly 
        IReadOnlyDictionary<string, TemperaturePreset> Catalogue = new 
        Dictionary<string, TemperaturePreset>(StringComparer.OrdinalIgnoreCase)
        {
            [Warm.Name] = Warm,
            [Cool.Name] = Cool,
            [Balanced.Name] = Balanced,
            [Off.Name] = Off,
        };
    
    /// <summary>
    /// Attempts to retrieve a preset by name.
    /// </summary>
    /// <param name="name">The preset name, matched case-insensitively.</param>
    /// <param name="preset">
    /// When this method returns <see langword="true"/>, contains the matching preset;
    /// otherwise <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/> if a matching preset was found.</returns>
    public static bool 
        TryGet(string name, out TemperaturePreset? preset) => Catalogue.TryGetValue(name, out preset);

    /// <summary>Gets a pipe-delimited string of all available preset names.</summary>
    public static string
        AvailableNames => string.Join("|", Catalogue.Keys);
}
