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
    
    public abstract Task<Type?> ExecuteAsync(object? payload = null);
    public abstract object? Payload { get; set; }
}