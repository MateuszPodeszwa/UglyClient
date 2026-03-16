namespace BeautifulClient.Utilities.ErrorHandler;

/// <summary>
/// Represents an immutable, strongly-typed error detailing why an operation failed.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To encapsulate failure states into a standardised format, completely eliminating the use of 'magic strings' or raw exceptions for application flow control.</para>
/// <para><b>Strategy:</b> Utilises a C# <c>record</c> to guarantee immutability and value-based equality. By exposing pre-defined, static instances for common network and HTTP failures, it ensures strict standardisation and DRY principles across the entire codebase.</para>
/// <para><b>Pattern:</b> Implements the Value Object pattern from Domain-Driven Design (DDD) and serves as the core failure payload within the broader Result pattern.</para>
/// </remarks>
public sealed record Error
{
    /// <summary>
    /// The unique, programmatic identifier for the error.
    /// </summary>
    public string Code { get; init; }
    
    /// <summary>
    /// The human-readable description of the error.
    /// </summary>
    public string Message { get; init; } 
    
    private Error(string code, string msg)
    {
        Code = code;
        Message = msg;
    }
    
    // Base / General
    public static Error None => new(string.Empty, string.Empty);
    public static Error NullValue => new("NullValue", "Provided value does not exist.");
    
    // Network & Connectivity
    public static Error Timeout => new("Network.Timeout", "The request timed out before receiving a response.");
    public static Error NetworkFailure => new("Network.Failure", "A network error occurred. Please check your connection.");
    
    // Standard HTTP Client Errors (4xx)
    public static Error BadRequest400 => new("Http.400", "Bad Request. The server could not understand the request.");
    public static Error Unauthorized401 => new("Http.401", "Unauthorised. Authentication is required or failed.");
    public static Error Forbidden403 => new("Http.403", "Forbidden. You do not have permission to access this resource.");
    public static Error NotFound404 => new("Http.404", "Not Found. The requested resource could not be found.");
    public static Error RequestTimeout408 => new("Http.408", "Request Timeout. The server timed out waiting for the request.");
    public static Error Conflict409 => new("Http.409", "Conflict. There is a conflict with the current state of the resource.");
    public static Error TooManyRequests429 => new("Http.429", "Too Many Requests. You have exceeded the rate limit.");

    // Standard HTTP Server Errors (5xx)
    public static Error InternalServerError500 => new("Http.500", "Internal Server Error. The API encountered an unexpected condition.");
    public static Error BadGateway502 => new("Http.502", "Bad Gateway. The server received an invalid response from the upstream server.");
    public static Error ServiceUnavailable503 => new("Http.503", "Service Unavailable. The API is currently offline or overloaded.");
    public static Error GatewayTimeout504 => new("Http.504", "Gateway Timeout. The upstream server failed to send a request in the time allowed.");

    // Payload & Parsing Errors
    public static Error EmptyResponse => new("Payload.Empty", "The API returned a successful status code, but the response body was empty.");
    public static Error InvalidJson => new("Payload.InvalidJson", "The API returned malformed or invalid JSON.");
    public static Error MappingError => new("Payload.MappingError", "Failed to map the JSON response to the expected object.");
    
    // Fallback for unlisted HTTP codes
    public static Error CustomHttpError(int statusCode, string reason) 
        => new($"Http.{statusCode}", $"API returned an error: {reason}");
}