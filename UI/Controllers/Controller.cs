using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.UI.Controllers;

public abstract class Controller : IRouter
{
    protected virtual IApiService Api { get; private set; }

    protected Controller(IApiService apiService)
    {
        this.Api =  apiService;
    }
    
    // Helper Methods for standardising & adapting returns
    protected ApiResult Ok() => ApiResult.Success();
    protected ApiResult<T> Ok<T>(T value) => value;
    protected ApiResult Fail(Error error) => (ApiResult) error;
    protected ApiResult<T> Fail<T>(Error error) => (ApiResult<T>) error;

    protected static TPayload PayloadAs<TPayload>(object? payload)
    {
        if (payload is TPayload typedPayload) return typedPayload;

        throw new InvalidCastException(
            $"Invalid payload type. Expected {typeof(TPayload).Name}, got {payload?.GetType().Name ?? "null"}.");
    }

    protected static TPayload PayloadAs<TPayload>(object? payload, TPayload defaultValue)
    {
        if (payload is null) return defaultValue;
        if (payload is TPayload typedPayload) return typedPayload;

        throw new InvalidCastException(
            $"Invalid payload type. Expected {typeof(TPayload).Name}, got {payload.GetType().Name}.");
    }
    
    public abstract Task<NavigationResult> ExecuteAsync(object? payload = null);
}
