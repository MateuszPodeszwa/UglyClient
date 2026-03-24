/* ========================================================================
 * Original Author : Mateusz Podeszwa
 * Contact         : podinatubie@gmail.com
 * Creation Date   : 2026-03-23
 * ------------------------------------------------------------------------
 * MAINTAINER NOTES & GOTCHAS:
 * ------------------------------------------------------------------------
 * - Methods accepting generics must define the constraint 
 *   `where T : IData` to prevent invalid or unexpected objects.
 * - `Id` and `RawJson` are strictly mandatory. Any additional 
 *   properties should act solely as DTO-specific data holders.
 * - To ensure an object tracks its history and supports 
 *   actions like `.SaveOnChangeAsync()`, it MUST inherit 
 *   from the `StatefulDto` abstract class.
 * - Any mutable field in a stateful DTO must notify changes 
 *   via its setter: `set => SetProperty(ref _field, value);`
 * ======================================================================== */

using BeautifulClient.Services.Api;

namespace BeautifulClient.Data.Objects;

/// <summary>
/// Represents a Data Transfer Object (DTO) containing sensor information. 
/// This object is returned whenever the API requests sensor-related data.
/// </summary>
public class SensorData : 
    StatefulDto<SensorData>,    // Defines history and states for the data object 
    IData                       // Establishes the “base” or minimal shape of each DTO
{
    /// <summary>
    /// The unique identifier for the sensor data record.
    /// </summary>
    /// <remarks>
    /// In a production environment, this is typically populated directly by the deserialized API response or database.
    /// 
    /// </remarks>
    public required int Id { get; init; }

    /// <summary>
    /// The unparsed, raw JSON string received from the API, preserved for debugging or secondary processing.
    /// </summary>
    public string? RawJson { get; set; }

    /// <summary>
    /// The recorded temperature value from the sensor.
    /// </summary>
    public celc Temperature
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Outputs the preserved JSON payload for this DTO.
    /// </summary>
    /// <returns>The <see cref="RawJson"/> string, or an empty string if the raw payload is null.</returns>
    public override string ToString()
    {
        // DtoActions is relying on this override.
        return RawJson ?? string.Empty;
    }
}