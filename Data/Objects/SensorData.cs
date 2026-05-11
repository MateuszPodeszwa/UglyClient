/* ========================================================================
 * Original Author : Mateusz Podeszwa
 * Contact         : podinatubie@gmail.com
 * Creation Date   : 2026-03-23
 * ======================================================================== */

using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.Pipelines;

namespace BeautifulClient.Data.Objects;

/// <summary>
/// Represents a Data Transfer Object (DTO) containing sensor information. 
/// This object is returned whenever the API requests sensor-related data.
/// </summary>
/// <remarks>
/// <para>
/// This class inherits from <see cref="StatefulDto{SensorData}"/> to enable change tracking, 
/// history management, and state persistence capabilities.
/// </para>
/// <para><b>Maintainer Notes &amp; Gotchas:</b></para>
/// <list type="bullet">
/// <item><description><b>Generic Constraints:</b> Methods accepting generics must define the constraint <c>where T : IData</c> to prevent invalid or unexpected objects.</description></item>
/// <item><description><b>Mandatory Fields:</b> <c>Id</c> and <c>RawJson</c> are strictly mandatory. Any additional properties act solely as DTO-specific data holders.</description></item>
/// <item><description><b>Statefulness:</b> To track history and support actions like <c>.SaveOnChangeAsync()</c>, the object MUST inherit from the <see cref="StatefulDto{T}"/> abstract class.</description></item>
/// <item><description><b>Mutation Tracking:</b> Any mutable field must notify changes via its setter. Furthermore, DTO-specific fields MUST use the injected <see cref="IObjectSetterPipeline"/> to track and log changes (e.g., <c>pipeline.Execute(() =&gt; SetProperty(...))</c>).</description></item>
/// </list>
/// </remarks>
public class SensorData(IObjectSetterPipeline pipeline) : 
    StatefulDto<SensorData>,    // Defines history and states for the data object 
    IData                       // Establishes the "base" or minimal shape of each DTO
{
    /// <summary>
    /// The unique identifier for the sensor data record.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is initialised via the constructor parameter and remains immutable throughout 
    /// the object's lifetime. It serves as the primary key for identifying this specific sensor data record.
    /// </para>
    /// <para>
    /// In a production environment, this is typically populated directly by the deserialised API response 
    /// or sourced from a database record. The value must be unique within the context of the system.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to supply the sensor ID from the method call 
    /// when mapping the API response:
    /// <code>
    /// public async Task&lt;ApiResult&lt;SensorData&gt;&gt; GetSensorTemperatureAsync(int sensorId)
    /// {
    ///     // Pass the raw, unexecuted method into the pipeline via a lambda.
    ///     return await apiResultPipeline.ExecuteAsync(() =&gt; GetAsync&lt;SensorData&gt;(
    ///         $"api/sensor/{sensorId}",
    ///         json =&gt; new(objectSetterPipeline)
    ///             {
    ///                 Id = sensorId,
    ///                 Temperature = json.GetDouble(), 
    ///                 RawJson = json.GetRawText(),
    ///                 SaveAction = () =&gt; throw new NotImplementedException("SaveAction for DTO States is not yet implemented") // [WIP] : This can be some algorithm.
    ///             }
    ///     ));
    /// }
    /// </code>
    /// </example>
    public required int Id { get; init; }
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
    /// The recorded temperature value from the sensor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property represents the temperature measurement captured by the associated sensor device. 
    /// The value is expressed in Celsius (°C) via the <see cref="celc"/> struct type.
    /// </para>
    /// <para>
    /// Property changes are tracked through the <see cref="IObjectSetterPipeline"/> to ensure 
    /// state changes are recorded, logged, and available for change history queries via 
    /// <see cref="StatefulDto{SensorData}"/>.
    /// </para>
    /// <para>
    /// <b>Implementation Gotcha:</b> When setting this property, the underlying state change 
    /// MUST be invoked through the injected pipeline using the backing field keyword: 
    /// <c>pipeline.Execute(() =&gt; SetProperty(ref field, value))</c>. This ensures dependents 
    /// are notified and the action is logged in the object's change history.
    /// </para>
    /// </remarks>
    public celc Temperature
    {
        get;
        set => pipeline.Execute((() => SetProperty(ref field, value)));
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