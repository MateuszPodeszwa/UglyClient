namespace BeautifulClient.Services.Api;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<string> GetAsync(string requestUri)
    {
        try
        {
            var response = await httpClient.GetAsync(requestUri);
        
            response.EnsureSuccessStatusCode();
        
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException)
        {
            // TODO: Consider Logger
            return "[Fallback] The API is currently offline or returning an error."; 
        }
    }
}