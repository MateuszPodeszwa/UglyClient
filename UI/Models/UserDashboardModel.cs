namespace BeautifulClient.UI.Models;

public class UserDashboardModel
{
    public int UserId { get; set; }
    public int SensorId { get; set; }
    public celc Temperature { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public object? Payload { get; set; } = null;
}