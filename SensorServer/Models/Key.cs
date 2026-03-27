
namespace Version2.Models
{
    public class ApiKeyManager
    {
        // key = API key sent by client
        // value = client id
        private static readonly Dictionary<string, string> _apiKeys = new()
        {
            { "u007-key",   "u007" },
            { "client3-key","client3" }
        };

        public bool ValidateApiKey(string apiKey)
        {
            // ✅ Check KEYS, not values
            return _apiKeys.ContainsKey(apiKey);
        }

        public string GetClientId(string apiKey)
        {
            // ✅ Look up by KEY, return the VALUE (clientId)
            return _apiKeys.TryGetValue(apiKey, out var clientId)
                ? clientId
                : null;
        }
    }
}

