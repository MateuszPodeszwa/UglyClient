using BeautifulClient.Data.Records;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Layouts;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities;
using Serilog;
using Spectre.Console;

namespace BeautifulClient.Tests.UI.Layouts;

public class MainLayoutTests
{
    [Fact]
    public async Task ReturnAsync_RendersLayoutAndDelegatesToInnerView()
    {
        SerilogQueSink.ClearLogs();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(new SerilogQueSink())
            .CreateLogger();
        logger.Information("layout-test-entry");

        var inner = new CapturingView();
        var subject = new MainLayout<string>(inner);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync("payload");
        string output = AnsiConsole.ExportText();

        Assert.Equal(1, inner.Calls);
        Assert.Equal("payload", inner.LastModel);
        Assert.Equal(typeof(string), result.NextRoute);
        Assert.Contains("BeautifulClient Dashboard", output);
        Assert.Contains("Activity Logs", output);
        Assert.Contains("layout-test-entry", output);
    }

    private sealed class CapturingView : IView<string>
    {
        public int Calls { get; private set; }
        public string? LastModel { get; private set; }

        public Task<NavigationResult> ReturnAsync(string model)
        {
            Calls++;
            LastModel = model;
            return Task.FromResult(new NavigationResult(typeof(string), model));
        }
    }
}
