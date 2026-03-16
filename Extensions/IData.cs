namespace BeautifulClient.Extensions;

/// <summary>
/// Defines the foundational contract for all data transfer objects (DTOs) and entities processed by the API or database.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To establish a guaranteed baseline shape for all domain models, ensuring every object has a unique identifier and retains its original payload for auditing or debugging.</para>
/// <para><b>Strategy:</b> Utilised primarily as a generic constraint (e.g., <c>where T : IData</c>). This provides strict compile-time safety, completely preventing developers from accidentally passing arbitrary, incompatible types into generic CRUD operations.</para>
/// <para><b>Pattern:</b> Implements the Base Entity / Layer Supertype pattern, acting as the structural root for all data models flowing through the system.</para>
/// </remarks>
public interface IData
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The unparsed, raw JSON string received from the API, preserved for debugging or secondary processing.
    /// </summary>
    public string? RawJson { get; init; }
}