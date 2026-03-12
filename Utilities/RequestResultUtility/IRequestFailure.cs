using System.Net;

namespace BeautifulClient.Utilities.RequestResultUtility;

internal interface IRequestFailure
{
    string? ErrorMessage { get; }
    HttpStatusCode? StatusCode { get; }
    Exception? Exception { get; }
}