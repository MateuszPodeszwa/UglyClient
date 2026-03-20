using BeautifulClient.Extensions;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Utilities;

/// <summary>
/// Defines the contract for a centralised execution pipeline that wraps API calls.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To abstract the execution of external requests, ensuring that cross-cutting concerns (such as logging, timing, and resilience) can be uniformly applied without coupling the calling code to a specific implementation.</para>
/// <para><b>Strategy:</b> Facilitates Dependency Inversion and significantly simplifies unit testing. By injecting this interface rather than a concrete class, downstream services can easily be tested using mocked pipelines that execute the delegate instantly, bypassing heavy logging or timing logic.</para>
/// <para><b>Pattern:</b> Represents the abstraction of the Decorator/Pipeline pattern, strictly defining how API execution delegates are intercepted and processed.</para>
/// </remarks>
public interface IApiPipeline
{
    public Task<ApiResult<TE>> ExecuteAsync<TE>(Func<Task<ApiResult<TE>>> apiCall);
    public Task<ApiResult> ExecuteAsync(Func<Task<ApiResult>> apiCall);
}