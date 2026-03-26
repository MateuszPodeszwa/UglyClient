using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BeautifulClient.Utilities.Pipelines;

/// <summary>
/// Defines a pipeline for executing and tracking property mutations within stateful objects.
/// </summary>
/// <remarks>
/// This interface is a core component of the state-tracking architecture. 
/// By wrapping property setters in this pipeline, the system can intercept, log, and 
/// process state changes consistently across all DTOs.
/// </remarks>
public interface IObjectSetterPipeline
{
    /// <summary>
    /// Executes the provided mutation function, allowing the pipeline to track and log the operation.
    /// </summary>
    /// <typeparam name="T">The return type of the underlying setter or operation.</typeparam>
    /// <param name="func">A delegate encapsulating the mutation logic (e.g., a call to <c>SetProperty</c>).</param>
    /// <returns>The result of the executed function.</returns>
    public T Execute<T>(Func<T> func);
}

/// <summary>
/// The default implementation of <see cref="IObjectSetterPipeline"/> that provides 
/// detailed diagnostic logging for property mutations.
/// </summary>
/// <remarks>
/// <para>
/// This class uses reflection to deeply inspect the delegate passed to it. When a lambda 
/// expression captures local variables (such as the <c>value</c> in a property setter), 
/// the C# compiler generates a hidden display class. 
/// </para>
/// <para>
/// By reflecting over the <see cref="Delegate.Target"/>, this pipeline can extract and log 
/// the exact values being set without requiring the caller to pass them as explicit parameters.
/// </para>
/// </remarks>
public class ObjectSetterPipeline(ILogger<ObjectSetterPipeline> logger) : IObjectSetterPipeline
{
    /// <inheritdoc/>
    public T Execute<T>(Func<T> func)
    {
        var method = func.Method;
        var target = func.Target;

        logger.LogInformation(
            """ 
            Setter Pipeline
                Set<{ReturnType}> (
                IsStatic={IsStatic}, 
                DeclaringType={TargetType}, 
                SetToValue={CapturedValues})
            """,
            typeof(T).Name,
            method.IsStatic,
            target?.GetType()?.DeclaringType?.Name.ToString() ?? "<none>",
            DescribeTargetValues(target));

        T result = func();

        logger.LogInformation(
            "Set<{ReturnType}> returned {ResultValue}",
            typeof(T).Name,
            FormatValue(result));

        return result;
    }

    /// <summary>
    /// Extracts and formats the captured variables from the compiler-generated closure class.
    /// </summary>
    /// <param name="target">The <see cref="Delegate.Target"/> object containing the captured variables.</param>
    /// <returns>A comma-separated string of the captured values, or an indicator if none are found.</returns>
    private static string DescribeTargetValues(object? target)
    {
        if (target is null)
        {
            return "<none>";
        }

        var fields = target
            .GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        return fields.Length == 0 
            ? "<no-fields>" 
            : string.Join(
                ", ",
                fields.Select(field => field.GetValue(target)));
    }

    /// <summary>
    /// Safely formats the result value for logging purposes, ensuring large strings are truncated.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A log-safe string representation of the value.</returns>
    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "<null>",
            string text => text.Length <= 120 ?
                $"\"{text}\"" :
                $"\"{text[..120]}...\"",
            ValueType => value.ToString() ?? value.GetType().Name,
            _ => value.ToString() ?? $"<{value.GetType().FullName}>"
        };
    }
}