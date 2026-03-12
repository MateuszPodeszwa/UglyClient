using BeautifulClient.Extensions;
namespace BeautifulClient.Data;

public class SensorData : IData
{
    public int Id { get; set; } = new Random().Next();
    public string? RawJson { get; init; }
    public double? Temperature { get; init; }
}