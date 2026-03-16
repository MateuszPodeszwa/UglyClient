using System.Text.Json;
using BeautifulClient.Extensions;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;

/// <summary>
/// Defines the contract for standard CRUD operations executed against an external API.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To abstract the raw HTTP communication, ensuring the calling client remains agnostic to network-level details.</para>
/// <para><b>Strategy:</b> Enforces the Single Responsibility Principle and Separation of Concerns by isolating HTTP request execution, JSON parsing, and exception handling away from the business logic.</para>
/// <para><b>Pattern:</b> Utilises the Result pattern to safely encapsulate and return either the mapped data or a handled error state, preventing exceptions from leaking to the caller.</para>
/// </remarks>
public interface IApiActions // TODO: Consider Abstract Class
{
    public Task<ApiResult<T>> GetAsync<T>(string requestUri, Func<JsonElement, T> createData) where T : IData;
    
    // public Task<ApiResult<T>> SendAsync<T>(string requestUri) where T : IData;
    // WIP
}