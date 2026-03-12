using BeautifulClient.Extensions;
using BeautifulClient.Utilities.RequestResultUtility;

namespace BeautifulClient.Models;

// Represents objects that can be returned by the HTTP GET function
public interface IRequestable
{
    IData? Data { get; init; }
    RequestResult Result { get; init; }
}