using BeautifulClient.Data.Records;
using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.UI.Controllers;
using BeautifulClient.Utilities;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Tests.UI.Controllers;

public class ControllerBaseTests
{
    [Fact]
    public void PayloadAs_ReturnsTypedPayload_WhenTypesMatch()
    {
        int value = TestController.ReadPayload<int>(5);

        Assert.Equal(5, value);
    }

    [Fact]
    public void PayloadAsWithDefault_ReturnsDefault_WhenPayloadIsNull()
    {
        int value = TestController.ReadPayloadWithDefault(null, 42);

        Assert.Equal(42, value);
    }

    [Fact]
    public void PayloadAs_ThrowsInvalidCast_WhenTypeDoesNotMatch()
    {
        var ex = Assert.Throws<InvalidCastException>(() => TestController.ReadPayload<int>("x"));

        Assert.Contains("Expected Int32", ex.Message);
    }

    [Fact]
    public void OkAndFailHelpers_CreateExpectedResultStates()
    {
        var controller = new TestController();

        var ok = controller.OkPublic();
        var okTyped = controller.OkTypedPublic("value");
        var fail = controller.FailPublic(Error.NetworkFailure);
        var failTyped = controller.FailTypedPublic<int>(Error.Timeout);

        Assert.True(ok.IsSuccess);
        Assert.True(okTyped.IsSuccess);
        Assert.Equal("value", okTyped.Value);
        Assert.True(fail.IsFailure);
        Assert.Equal(Error.NetworkFailure, fail.Error);
        Assert.True(failTyped.IsFailure);
        Assert.Equal(Error.Timeout, failTyped.Error);
    }

    private sealed class TestController() : Controller(new FakeApiService())
    {
        public static T ReadPayload<T>(object? payload) => PayloadAs<T>(payload);

        public static T ReadPayloadWithDefault<T>(object? payload, T defaultValue) =>
            PayloadAs(payload, defaultValue);

        public ApiResult OkPublic() => Ok();

        public ApiResult<T> OkTypedPublic<T>(T value) => Ok(value);

        public ApiResult FailPublic(Error error) => Fail(error);

        public ApiResult<T> FailTypedPublic<T>(Error error) => Fail<T>(error);

        public override Task<NavigationResult> ExecuteAsync(object? payload = null) =>
            Task.FromResult(new NavigationResult(null));
    }
}
