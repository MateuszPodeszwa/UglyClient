using System.Net;

namespace BeautifulClient.Utilities.RequestResultUtility;

public abstract record RequestResult
{
    private static readonly RequestResult SuccessInstance = new SuccessResult();
    
    private RequestResult() { }
    
    public bool IsSuccess => this is IRequestSuccess; 
    public static RequestResult Success() => SuccessInstance;
    
    public static RequestResult Failure(
        string? errorMessage,
        HttpStatusCode? statusCode,
        Exception? exception = null
    ) => new FailureResult(errorMessage, statusCode, exception);
    
    private sealed record SuccessResult : RequestResult, IRequestSuccess;
    private sealed record FailureResult(
        string? ErrorMessage,
        HttpStatusCode? StatusCode,
        Exception? Exception = null
    ) : RequestResult, IRequestFailure;
}