using System.Text.Json;
using BeautifulClient.Extensions;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;

/// <summary>
/// Provides a foundational, reusable implementation of the <see cref="IApiActions"/> interface for executing generic API requests.
/// </summary>
/// <param name="httpClient">The efficiently managed HTTP client instance injected via Dependency Injection and passed down from the derived typed client.</param>
/// <remarks>
/// <para><b>Purpose:</b> To centralise the repetitive boilerplate of HTTP communication, JSON parsing, and low-level exception handling into one common location.</para>
/// <para><b>Strategy:</b> Uses inheritance to encapsulate standard network operations away from domain-specific services (like hardware sensors). Methods are marked as <c>virtual</c> to provide a robust default implementation while allowing derived classes to override the behaviour for non-standard endpoints.</para>
/// <para><b>Pattern:</b> Implements the Base Class / Template Method pattern. It executes the core algorithm for fetching and parsing data, whilst delegating the specific JSON-to-Object mapping to the caller via the <c>createData</c> function delegate.</para>
/// </remarks>
public abstract class ApiActions(HttpClient httpClient) : IApiActions
{
    // ReSharper disable once MemberCanBePrivate.Global
    public virtual async Task<ApiResult<T>> GetAsync<T>(string requestUri, Func<JsonElement, T> createData) where T : IData
    {
        try
        {
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUri);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return (ApiResult<T>) responseMessage.ToError(); // Maps some of the pre-defined errors to ApiResult.Failure().
            }
        
            var responseMessageContent = await responseMessage.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseMessageContent);

            // The implicit operator automatically wraps this T in ApiResult<T>.Success()
            return createData(doc.RootElement); 
        }
        catch (HttpRequestException)
        {
            return (ApiResult<T>) Error.NetworkFailure;
        }
        catch (JsonException)
        {
            return (ApiResult<T>) Error.InvalidJson;
        }
    }
}