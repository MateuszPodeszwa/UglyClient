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