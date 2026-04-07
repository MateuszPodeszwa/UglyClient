using BeautifulClient.Utilities.Pipelines;

namespace BeautifulClient.Tests.UI.TestDoubles;

internal sealed class PassThroughObjectSetterPipeline : IObjectSetterPipeline
{
    public static PassThroughObjectSetterPipeline Instance { get; } = new();

    public T Execute<T>(Func<T> func) => func();
}
