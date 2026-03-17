// ReSharper disable MemberCanBePrivate.Global

using BeautifulClient.Extensions;

namespace BeautifulClient.Utilities.ErrorHandler;

/// <summary>
   /// Represents the outcome of an operation, encapsulating either a successful execution or a detailed error state.
   /// </summary>
   /// <remarks>
   /// <para><b>Purpose:</b> To provide a standard response contract across the codebase, avoiding computationally heavy exceptions for expected operational failures.</para>
   /// <para><b>Strategy:</b> Exposes immutable properties that strictly bind a boolean success flag to a specific <see cref="ErrorHandler.Error"/> instance. Uses protected constructors to force object creation through intentional factory methods, ensuring invalid states cannot be instantiated.</para>
   /// <para><b>Pattern:</b> Implements the Result pattern, serving as the foundational wrapper for controlling application flow without relying on <c>try-catch</c> blocks for domain logic.</para>
   /// </remarks>
public class ApiResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    
    // Non-nullable, use Error.None for successes
    public Error Error { get; }

    // Protected so ApiResult<T> can inherit from it later if needed
    protected ApiResult(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static ApiResult Success() => new(true, Error.None);
    public static ApiResult Failure(Error error) => new(false, error);
    
    public static explicit operator ApiResult(Error error) => Failure(error);
}

/// <summary>
/// Represents the outcome of a data-returning operation, encapsulating either the requested payload or a detailed error state.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
/// <remarks>
/// <para><b>Purpose:</b> To safely transport data across system boundaries while guaranteeing that the caller handles potential failures before accessing the payload.</para>
/// <para><b>Strategy:</b> Inherits the baseline state from <see cref="ApiResult"/> but guards the <see cref="Value"/> property. If accessed during a failure state, it intentionally throws an exception to immediately catch logical developer errors. Includes conversion operators for cleaner syntax in the service layer.</para>
/// <para><b>Pattern:</b> Implements the generic Result pattern (a Monad-like structure), providing safe encapsulation and unboxing of data.</para>
/// </remarks>
public sealed class ApiResult<T> : ApiResult
{
    private ApiResult(T? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        // TODO: This needs re-visiting.
        Value = value!;
    }

    public T Value { get; }
    
    public static ApiResult<T> Success(T value) => new(value, true, Error.None);
    public new static ApiResult<T> Failure(Error error) => new(default, false, error);
    
    public static explicit operator ApiResult<T>(Error error) => Failure(error);
    public static implicit operator ApiResult<T>(T value) => Success(value);
}