namespace BeautifulClient.Extensions;

public interface IData
{
    public int Id { get; set; }
    public string? RawJson { get; init; }
}