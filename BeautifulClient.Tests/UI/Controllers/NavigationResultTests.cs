using BeautifulClient.Data.Records;
using BeautifulClient.UI.Controllers;
using Xunit;

namespace BeautifulClient.Tests.UI.Controllers;

public class NavigationResultTests
{
	[Fact]
	public void Constructor_PreservesRouteAndPayload_WhenValuesProvided()
	{
		Type route = typeof(string);
		object payload = 42;

		var result = new NavigationResult(route, payload);

		Assert.Equal(route, result.NextRoute);
		Assert.Equal(payload, result.Payload);
	}

	[Fact]
	public void Constructor_UsesNullPayloadByDefault_WhenPayloadIsOmitted()
	{
		Type route = typeof(int);

		var result = new NavigationResult(route);

		Assert.Equal(route, result.NextRoute);
		Assert.Null(result.Payload);
	}

	[Fact]
	public void Constructor_AllowsNullRoute_WhenNoNextRouteIsProvided()
	{
		var result = new NavigationResult(null, "payload");

		Assert.Null(result.NextRoute);
		Assert.Equal("payload", result.Payload);
	}
}

public class NavigationResultOfTPayloadTests
{
	[Fact]
	public void Constructor_PreservesRouteAndTypedPayload_WhenValuesProvided()
	{
		Type route = typeof(Guid);
		var result = new NavigationResult<string>(route, "user-123");

		Assert.Equal(route, result.NextRoute);
		Assert.Equal("user-123", result.Payload);
	}

	[Fact]
	public void Constructor_UsesDefaultPayload_WhenPayloadIsOmitted()
	{
		Type route = typeof(decimal);

		var result = new NavigationResult<int>(route);

		Assert.Equal(route, result.NextRoute);
		Assert.Equal(0, result.Payload);
	}

	[Fact]
	public void ImplicitConversion_PreservesRouteAndPayload_WhenConvertedToNonGenericType()
	{
		Type route = typeof(DateTime);
		NavigationResult generic = new NavigationResult<string>(route, "abc");

		Assert.Equal(route, generic.NextRoute);
		Assert.Equal("abc", generic.Payload);
	}

	[Fact]
	public void ImplicitConversion_HandlesNullRouteAndNullPayload_WhenConvertedToNonGenericType()
	{
		NavigationResult generic = new NavigationResult<string>(null, null);

		Assert.Null(generic.NextRoute);
		Assert.Null(generic.Payload);
	}
}

