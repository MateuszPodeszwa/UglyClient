using BeautifulClient.Data.Records;
using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.Services.Api;
using BeautifulClient.UI;
using BeautifulClient.UI.Controllers;
using BeautifulClient.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace BeautifulClient.Tests.UI;

public class ConsoleHostTests
{
    [Fact]
    public async Task Host_PassesInitialSettingsPayload_ToFirstController()
    {
        ServiceProvider services = BuildServices();
        var host = new ConsoleHost(services);
        var recorder = services.GetRequiredService<ExecutionRecorder>();

        await host.Host<FirstController>(settings: 77);

        Assert.Equal([77], recorder.FirstPayloads);
    }

    [Fact]
    public async Task Host_FollowsRoutesAndPropagatesReturnedPayload()
    {
        ServiceProvider services = BuildServices();
        var host = new ConsoleHost(services);
        var recorder = services.GetRequiredService<ExecutionRecorder>();

        await host.Host<FirstController>(settings: 10);

        Assert.Equal([10], recorder.FirstPayloads);
        Assert.Equal([11], recorder.SecondPayloads);
    }

    private static ServiceProvider BuildServices()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<ExecutionRecorder>();
        collection.AddSingleton<IApiService, FakeApiService>();
        collection.AddTransient<FirstController>();
        collection.AddTransient<SecondController>();
        return collection.BuildServiceProvider();
    }

    private sealed class ExecutionRecorder
    {
        public List<int> FirstPayloads { get; } = [];
        public List<int> SecondPayloads { get; } = [];
    }

    private sealed class FirstController(IApiService api, ExecutionRecorder recorder) : Controller(api)
    {
        public override Task<NavigationResult> ExecuteAsync(object? payload = null)
        {
            recorder.FirstPayloads.Add(PayloadAs(payload, -1));
            return Task.FromResult(new NavigationResult(typeof(SecondController), 11));
        }
    }

    private sealed class SecondController(IApiService api, ExecutionRecorder recorder) : Controller(api)
    {
        public override Task<NavigationResult> ExecuteAsync(object? payload = null)
        {
            recorder.SecondPayloads.Add(PayloadAs(payload, -1));
            return Task.FromResult(new NavigationResult(null));
        }
    }
}
