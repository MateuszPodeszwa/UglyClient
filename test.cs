namespace BeautifulClient;

public interface IMessageService
{
    void SendMessage(string message);
}

public class MessageService : IMessageService
{
    public void SendMessage(string message)
    {
        Console.WriteLine($"Message sent: {message}");
    }
}

public class MySettings
{
    public string GreetingMessage { get; set; } = string.Empty;
}