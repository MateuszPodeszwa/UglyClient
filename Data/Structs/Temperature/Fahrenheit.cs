// ReSharper disable All

namespace BeautifulClient.Data.Structs.Temperature;

[Obsolete("[WIP] Not Implemented")]
public record struct Fahrenheit(double Value) : IComparable<Fahrenheit>, IFormattable, ITemperature
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(Value)}: {Value}";
        return formattable.ToString(formatProvider);
    }

    public int CompareTo(Fahrenheit other)
    {
        throw new NotImplementedException();
    }

    public readonly override string ToString()
    {
        return $"{nameof(Value)}: {Value}";
    }

    public Celcius ToCelsius()
    {
        throw new NotImplementedException();
    }

    public Fahrenheit ToFahrenheit()
    {
        throw new NotImplementedException();
    }
}