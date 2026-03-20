using BeautifulClient.Extensions;

namespace BeautifulClient.Data;

public class FanData : IData
{
    public int Id { get; set; }
    public string? RawJson { get; init; }
    public override string ToString()
    {
        // DtoActions is relying on this override.
        return RawJson ?? string.Empty;
    }
}