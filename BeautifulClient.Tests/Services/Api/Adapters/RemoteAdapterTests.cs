using System.Net;
using System.Text;
using BeautifulClient.Services.Api.Adapters;
using BeautifulClient.Utilities.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeautifulClient.Tests.Services.Api.Adapters;

public class RemoteAdapterTests
{
    [Fact]
    public async Task GetHeaterDataAsync_UsesHeatControllerLevelRoute_AndReturnsParsedLevel()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            if (request.Method == HttpMethod.Get &&
                request.RequestUri?.AbsolutePath == "/api/heat/2/level")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("3", Encoding.UTF8, "application/json")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        var subject = CreateSubject(handler);

        var result = await subject.GetHeaterDataAsync(2);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Id);
        Assert.Equal(3, result.Value.Level);
        Assert.Equal("/api/heat/2/level", handler.LastRequestPath);
    }

    private static RemoteAdapter CreateSubject(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5077")
        };

        return new RemoteAdapter(
            client,
            new ApiResultPipeline(NullLogger<ApiResultPipeline>.Instance),
            new ObjectSetterPipeline(NullLogger<ObjectSetterPipeline>.Instance));
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public string? LastRequestPath { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestPath = request.RequestUri?.AbsolutePath;
            return Task.FromResult(responder(request));
        }
    }
}
