using BeautifulClient.Services.Api.Adapters;
using BeautifulClient.Utilities.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeautifulClient.Tests.Services.Api.Adapters;

public class LocalAdapterTests
{
    [Fact]
    public async Task GetSensorTemperatureAsync_ReturnsSuccess_ForValidLocalSensorId()
    {
        var subject = new LocalAdapter(
            new HttpClient(),
            new ApiResultPipeline(NullLogger<ApiResultPipeline>.Instance),
            new ObjectSetterPipeline(NullLogger<ObjectSetterPipeline>.Instance));

        var result = await subject.GetSensorTemperatureAsync(4);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value.Id);
    }
}
