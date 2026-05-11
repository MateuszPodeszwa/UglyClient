using System.Net.Http.Json;
using System.Text.Json;
using BeautifulClient.Data;
using BeautifulClient.Utilities.ErrorHandler;
using BeautifulClient.Utilities.Extensions;

namespace BeautifulClient.Services.Api.Actions;

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
    // createData works as low-level adapter, it forces one to define way the data transfer object is created
    public virtual async Task<ApiResult<T>> GetAsync<T>(string requestUri, Func<JsonElement, T> createData) where T : IData
    {
        try
        {
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(requestUri);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return
                    (ApiResult<T>)responseMessage
                        .ToError(); // Maps some of the pre-defined errors to ApiResult.Failure().
            }

            var responseMessageContent = await responseMessage.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseMessageContent);

            // The implicit operator automatically wraps this T in ApiResult<T>.Success()
            return createData(doc.RootElement);
        }
        // Absolutely do not include any Console.WriteLine in those catch
        catch (HttpRequestException e)
        {
            return (ApiResult<T>)Error.NetworkFailure;
        }
        catch (JsonException)
        {
            return (ApiResult<T>)Error.InvalidJson;
        }
        catch (Exception exception)
        {
            return (ApiResult<T>)Error.CustomHttpError(exception.HResult, exception.Message);
        }
    }

    /// <summary>
    /// Sends an HTTP POST request with no body to the specified URI and returns an <see cref="ApiResult"/>.
    /// </summary>
    /// <param name="requestUri">The endpoint URI to post to.</param>
    /// <returns>An <see cref="ApiResult"/> indicating success or a mapped failure state.</returns>
    public virtual async Task<ApiResult> PostEmptyAsync(string requestUri)
    {
        try
        {
            using HttpResponseMessage responseMessage = await httpClient.PostAsync(requestUri, content: null);

            if (!responseMessage.IsSuccessStatusCode)
                return (ApiResult)responseMessage.ToError();

            return ApiResult.Success();
        }
        catch (HttpRequestException)
        {
            return (ApiResult)Error.NetworkFailure;
        }
        catch (Exception exception)
        {
            return (ApiResult)Error.CustomHttpError(exception.HResult, exception.Message);
        }
    }

    // Set function that returns no value back, only confirmation.
    public async Task<ApiResult> SetAsync<TRequest>(string requestUri, TRequest payload)
    {
        try
        {
            using HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync(requestUri, payload);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return (ApiResult) responseMessage.ToError();
            }

            return ApiResult.Success();
        }
        catch (HttpRequestException)
        {
            return (ApiResult)Error.NetworkFailure;
        }
        catch (JsonException)
        {
            return (ApiResult)Error.InvalidJson;
        }
        catch (Exception exception)
        {
            return (ApiResult)Error.CustomHttpError(exception.HResult, exception.Message);
        }
    }
}