namespace BeautifulClient.Services.Api;

public class ApiService(HttpClient httpClient) : IApiService
{
    public async Task<string> GetAsync()
    {
        var response = await httpClient.GetAsync("data");
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}