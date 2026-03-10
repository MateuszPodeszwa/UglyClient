namespace BeautifulClient.Services.Api;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<string> GetAsync(string requestUri)
    {
        var response = await httpClient.GetAsync(requestUri);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}