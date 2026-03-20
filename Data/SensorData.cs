using BeautifulClient.Extensions;

namespace BeautifulClient.Data;

/// <summary>
/// Represents a Data Transfer Object (DTO) containing sensor information. 
/// This object is returned whenever the API requests sensor-related data.
/// </summary>
public class SensorData : IData
{
    /// <summary>
    /// The unique identifier for the sensor data record.
    /// </summary>
    /// <remarks>
    /// In a production environment, this is typically populated directly by the deserialized API response or database.
    /// 
    /// </remarks>
    public required int Id { get; set; }

    /// <summary>
    /// The unparsed, raw JSON string received from the API, preserved for debugging or secondary processing.
    /// </summary>
    public string? RawJson { get; init; }

    /// <summary>
    /// The recorded temperature value from the sensor.
    /// </summary>
    public celc? Temperature { get; init; }
    
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