using BeautifulClient.Data.Objects;
using BeautifulClient.Utilities.ErrorHandler;
using BeautifulClient.Utilities.Pipelines;
using Xunit;

namespace BeautifulClient.Tests.Data.Objects;

public class FanDataTests
{
	[Fact]
	public void ToString_ReturnsEmptyString_WhenRawJsonIsNull()
	{
		FanData fanData = CreateSubject();

		string result = fanData.ToString();

		Assert.Equal(string.Empty, result);
	}

	[Fact]
	public void ToString_ReturnsRawJson_WhenRawJsonIsSet()
	{
		FanData fanData = CreateSubject();
		fanData.RawJson = "{\"status\":true}";

		string result = fanData.ToString();

		Assert.Equal("{\"status\":true}", result);
	}

	[Fact]
	public void Status_UsesObjectSetterPipeline_AndAppliesNewValue()
	{
		var pipeline = new SpyObjectSetterPipeline();
		FanData fanData = CreateSubject(pipeline);

		fanData.Status = true;

		Assert.True(fanData.Status);
		Assert.Equal(1, pipeline.ExecuteCalls);
	}

	private static FanData CreateSubject(IObjectSetterPipeline? pipeline = null)
	{
		return new FanData(pipeline ?? new PassThroughObjectSetterPipeline())
		{
			SaveAction = _ => Task.FromResult(ApiResult.Success())
		};
	}

	private sealed class PassThroughObjectSetterPipeline : IObjectSetterPipeline
	{
		public T Execute<T>(Func<T> func) => func();
	}

	private sealed class SpyObjectSetterPipeline : IObjectSetterPipeline
	{
		public int ExecuteCalls { get; private set; }

		public T Execute<T>(Func<T> func)
		{
			ExecuteCalls++;
			return func();
		}
	}
}


