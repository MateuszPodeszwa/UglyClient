using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace BeautifulClient.Utilities;

public class HttpPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger<HttpPolicies> logger)
    {
        // HandleTransientHttpError automatically handles HTTP 5xx errors and HTTP 408 (Request Timeout)
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3, // Total retries
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    var debug = $"API call failed. Waiting {timespan.TotalSeconds} seconds before retry #{retryAttempt}.";
                    logger.LogWarning(debug);
                });
    }
}