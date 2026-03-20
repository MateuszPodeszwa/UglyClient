using System.Net;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpResponseMessage"/> to seamlessly map network responses into the application's unified <see cref="Error"/> domain model.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To automate the translation of generic HTTP status codes into strongly-typed, predictable error states that the rest of the codebase can safely process.</para>
/// <para><b>Strategy:</b> Utilises C# extension methods to graft this mapping functionality directly onto the framework's native HTTP response class. This keeps the calling API services DRY and entirely free of repetitive switch statements.</para>
/// <para><b>Pattern:</b> Acts as an Adapter/Translator, bridging the gap between the low-level HTTP transport layer and the application's custom Result pattern.</para>
/// </remarks>
public static class HttpResponseMessageExtension
{
    /// <summary>
    /// Evaluates the HTTP response and converts any non-success status code into a specific <see cref="Error"/> record.
    /// </summary>
    /// <param name="response">The HTTP response message to evaluate.</param>
    /// <returns>An <see cref="Error.None"/> if successful, or a strongly-typed <see cref="Error"/> corresponding to the HTTP status code.</returns>
    public static Error ToError(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Error.None;
        }

        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => Error.BadRequest400,
            HttpStatusCode.Unauthorized => Error.Unauthorized401,
            HttpStatusCode.Forbidden => Error.Forbidden403,
            HttpStatusCode.NotFound => Error.NotFound404,
            HttpStatusCode.RequestTimeout => Error.RequestTimeout408,
            HttpStatusCode.Conflict => Error.Conflict409,
            HttpStatusCode.TooManyRequests => Error.TooManyRequests429,
            
            HttpStatusCode.InternalServerError => Error.InternalServerError500,
            HttpStatusCode.BadGateway => Error.BadGateway502,
            HttpStatusCode.ServiceUnavailable => Error.ServiceUnavailable503,
            HttpStatusCode.GatewayTimeout => Error.GatewayTimeout504,
            HttpStatusCode.UnsupportedMediaType => Error.UnsupportedMediaType,
            
            // Fallback for anything else (e.g., 067 I'm a teapot, hehe)
            _ => Error.CustomHttpError((int)response.StatusCode, response.ReasonPhrase ?? "Unknown Error")
        };
    }
}