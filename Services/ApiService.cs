namespace BeautifulClient.Services;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<string> GetAsync()
    {
        // Because BaseAddress is pre-set, you only need the relative path
        var response = await httpClient.GetAsync("data");
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}