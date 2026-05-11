using System.Diagnostics;
using BeautifulClient.Services.Api.Actions;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging;

// ReSharper disable SuggestVarOrType_Elsewhere

namespace BeautifulClient.Utilities.Pipelines;

/// <summary>
/// Acts as a centralised execution pipeline for all API calls within the application.
/// </summary>
/// <param name="logger">The logger instance used to uniformly record the outcome and duration of requests.</param>
/// <remarks>
/// <para><b>Purpose:</b> To provide a single, global checkpoint for executing API requests, allowing cross-cutting concerns (like telemetry and logging) to be handled in one place.</para>
/// <para><b>Strategy:</b> Enforces the Single Responsibility and DRY (Don't Repeat Yourself) principles. By extracting logging and execution timing into this pipeline, downstream service classes are relieved of repetitive boilerplate. It also establishes a single choke point, making it trivial to introduce global resilience features in the future.</para>
/// <para><b>Pattern:</b> Implements the Pipeline/Decorator pattern. It wraps the execution of the provided delegate with pre- and post-processing logic (stopwatch and logging) without modifying the delegate's behaviour.</para>
/// </remarks>
public class ApiResultPipeline(ILogger<ApiResultPipeline> logger) : IApiPipeline
{
    public async Task<ApiResult<TE>> ExecuteAsync<TE>(Func<Task<ApiResult<TE>>> apiCall) 
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        ApiResult<TE> result = await apiCall();

        stopwatch.Stop();
        var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        if (result.IsFailure)
        {
            logger.LogWarning("Operation failed after {ElapsedMs}ms | Code: {ErrorCode} | Message: {ErrorMessage}",
                elapsedMs,
                result.Error.Code,
                result.Error.Message);
        }
        else 
        {
            logger.LogInformation("Operation succeeded in {ElapsedMs}ms", elapsedMs);
            logger.LogInformation("Returned {Result} with contents {ToString}", result?.Value?.GetType().Name, result?.Value?.ToString());
        }
        
        return result!;
    }

    public async Task<ApiResult> ExecuteAsync(Func<Task<ApiResult>> apiCall)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        ApiResult result = await apiCall();

        stopwatch.Stop();
        var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
        if (result.IsFailure)
        {
            logger.LogWarning("Operation failed after {ElapsedMs}ms | Code: {ErrorCode} | Message: {ErrorMessage}",
                elapsedMs,
                result.Error.Code,
                result.Error.Message);
        }
        else 
        {
            logger.LogInformation("Operation succeeded in {ElapsedMs}ms", elapsedMs);
            logger.LogInformation("Returned {Result} with no contents", result.GetType().Name);
        }
        
        return result;
    }
}