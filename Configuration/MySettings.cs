namespace BeautifulClient.Configuration;

public record MySettings
{
    public string GreetingMessage { get; set; } = string.Empty;
    public string TotalRetries { get; set; } = string.Empty;
}