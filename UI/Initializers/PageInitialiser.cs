using BeautifulClient.Services.Api;
using Spectre.Console;

namespace BeautifulClient.UI.Initializers;

public abstract class PageInitialiser : IRenderable
{
    public async Task RunAsync()
    {
        AnsiConsole.Clear();
        RenderHeader();
        await RenderMainContentAsync(); // Child class handles this
        RenderLogs();
    }
    
    protected abstract Task RenderMainContentAsync();

    protected IApiService ApiService { get; }
    private Queue<string> ActivityLogs { get; }

    protected virtual int MaxLogs { get; set; }
    protected virtual void AddLog(string message)
    {
        if (ActivityLogs.Count >= MaxLogs)
        {
            ActivityLogs.Dequeue();
        }
        
        ActivityLogs.Enqueue($"[[{DateTime.Now:HH:mm:ss}]] {message}");
    }

    // ReSharper disable once ConvertToPrimaryConstructor
    protected PageInitialiser(IApiService apiService)
    {
        ApiService = apiService;
        ActivityLogs = new Queue<string>();
    }
    
    protected virtual void RenderHeader()
    {
        AnsiConsole.Write(new Rule("[blue]BeautifulClient Dashboard[/]").Centered());
    }
    
    protected virtual void RenderLogs()
    {
        AnsiConsole.Write(new Rule("[grey]Activity Logs[/]").LeftJustified());
        foreach (var log in ActivityLogs)
        {
            AnsiConsole.MarkupLine($"[grey]{log}[/]");
        }
    }
}