namespace BeautifulClient.Data.Objects;

[Obsolete]
public class HeaterData : IData
{
    public int Id { get; init; }
    public string? RawJson { get; set; }
    
    public override string ToString()
    {
        // DtoActions is relying on this override.
        return RawJson ?? string.Empty;
    }
}