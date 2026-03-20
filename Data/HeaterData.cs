using BeautifulClient.Extensions;

namespace BeautifulClient.Data;

public class HeaterData : IData
{
    public int Id { get; set; }
    public string? RawJson { get; init; }
    public override string ToString()
    {
        return RawJson ?? string.Empty;
    }
}