namespace BeautifulClient.Data.Records;

public record NavigationResult(Type? NextRoute, object? Payload = null);

public sealed record NavigationResult<TPayload>(Type? NextRoute, TPayload? Payload = default)
{
	public static implicit operator NavigationResult(NavigationResult<TPayload> value)
		=> new(value.NextRoute, value.Payload);
}
