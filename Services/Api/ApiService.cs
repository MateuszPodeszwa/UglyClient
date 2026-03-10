using Microsoft.Extensions.Logging;

namespace BeautifulClient.Services.Api;

public class ApiService(HttpClient httpClient, ILogger<ApiService> logger) : IApiService
{
    public async Task<string> GetAsync(string requestUri)
    {
        HttpResponseMessage responseMessage = null!;
        try
        {
            responseMessage = await httpClient.GetAsync(requestUri);
            responseMessage.EnsureSuccessStatusCode();
            return await responseMessage.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            #pragma warning disable CA1873
            logger.LogCritical("Request failed: {HttpResponseMessage}", responseMessage);
            #pragma warning restore CA1873
            
            return responseMessage.ReasonPhrase ??  "Request failed"; 
        }
    }
}