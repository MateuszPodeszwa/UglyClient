using BeautifulClient.Extensions;
using BeautifulClient.Utilities.RequestResultUtility;

namespace BeautifulClient.Models;

public sealed class ApiRequest : IRequestable
{
    public IData? Data { get; init; }
    public RequestResult Result { get; init; }

    public bool IsSuccess => this.Result.IsSuccess;

    private ApiRequest(IData? data, RequestResult result)
    {
        this.Data = data;
        this.Result = result;
    }

    public static ApiRequest Create(RequestResult r, IData? d = null) => new(d, r);
}