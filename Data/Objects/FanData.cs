using BeautifulClient.Utilities.Pipelines;

namespace BeautifulClient.Data.Objects;

public class FanData(IObjectSetterPipeline objectSetterPipeline) : 
    StatefulDto<FanData>,
    IData
{
    /// <inheritdoc/>
    public int Id { get; init; }
    /// <summary>
    /// The unparsed, raw JSON string received from the API, preserved for debugging or secondary processing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property stores the original JSON payload returned by the API endpoint before any 
    /// deserialisation or transformation occurs. This is useful for:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Debugging API response format changes or discrepancies</description></item>
    /// <item><description>Re-deserialising the payload with alternative parsers or settings</description></item>
    /// <item><description>Auditing or logging the exact data received from external services</description></item>
    /// <item><description>Supporting data migration or transformation operations</description></item>
    /// </list>
    /// <para>
    /// May be <c>null</c> if the API response was not captured or the data was synthesised locally.
    /// </para>
    /// </remarks>
    public string? RawJson { get; set; }
    /// <summary>
    /// Gets or sets the fan operating state.
    /// </summary>
    /// <remarks>
    /// The setter routes updates through <c>objectSetterPipeline.Execute</c> so that
    /// state changes are applied via <c>SetProperty</c>, enabling centralized tracking,
    /// logging, and change-notification behavior.
    /// </remarks>
    public bool Status
    {
        get;
        set => objectSetterPipeline.Execute((() => SetProperty(ref field, value)));
    }
    /// <summary>
    /// Outputs the preserved JSON payload for this DTO.
    /// </summary>
    /// <returns>
    /// The <see cref="RawJson"/> string if available; otherwise, an empty string.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This override is used by the data access/action layer (DtoActions) to retrieve 
    /// the original API response for serialization, re-processing, or transmission to downstream consumers.
    /// </para>
    /// <para>
    /// If <see cref="RawJson"/> is <c>null</c>, this method returns <see cref="string.Empty"/> 
    /// to avoid null propagation and provide a predictable, safe default value.
    /// </para>
    /// </remarks>
    public override string ToString()
    {
        // DtoActions is relying on this override.
        return RawJson ?? string.Empty;
    }
}