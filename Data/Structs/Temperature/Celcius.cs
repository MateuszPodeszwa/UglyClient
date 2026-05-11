using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BeautifulClient.Data.Structs.Temperature;

[SuppressMessage("ReSharper", "UseSymbolAlias")]
public readonly record struct Celcius(double Value) : IComparable<Celcius>, IFormattable, ITemperature
{

    // Makes implicit conversion, Celcius x = 32.3;
    public static implicit operator Celcius(double d) => new(d);

    // Allows for the reverse explicit conversion to double, e.g., (double) y.Celcius();
    public static explicit operator double(Celcius t) => t.Value;
    
    // The default ToString behaviour, prints {Value) °C
    public override string ToString() => ToString("G", CultureInfo.InvariantCulture);
    
    // Define the base behaviour
    public string ToString(string? format, IFormatProvider? formatProvider) => $"{Value.ToString(format, formatProvider)} °C";

    // Comparison logic
    public int CompareTo(Celcius other) => Value.CompareTo(other.Value);

    public static bool operator <(Celcius left, Celcius right) => left.Value < right.Value;
    public static bool operator >(Celcius left, Celcius right) => left.Value > right.Value;
    public static bool operator <=(Celcius left, Celcius right) => left.Value <= right.Value;
    public static bool operator >=(Celcius left, Celcius right) => left.Value >= right.Value;

    // Basic arithmetic for adding & subtracting temperature values
    public static Celcius operator +(Celcius left, Celcius right) => new(left.Value + right.Value);
    public static Celcius operator -(Celcius left, Celcius right) => new(left.Value - right.Value);

    // Parsing logic
    public static Celcius Parse(string s, IFormatProvider? provider = null)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (TryParse(s, provider, out Celcius result))
        {
            return result;
        }
        throw new FormatException($"String '{s}' was not recognised as a valid Celcius.");
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // Try to convert a string such 43 °C or 23 etc... into a valid Celcius type
    public static bool TryParse(
        [NotNullWhen(true)] string? s // GET RID OF THESE "OH IT MAY BE NULL!!!" Errors
        , IFormatProvider? provider, 
        out Celcius result)
    {
        // Validate if the passed variable has something meaningfull in it
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        // Clean up the string by removing the unit if it exists
        var cleanString = s.Replace("°C", string.Empty).Replace("C", string.Empty).Replace("° C", string.Empty).Trim();
        
        // Parse the double and explicitly instantiate a new Celcius object
        if (double.TryParse(cleanString, NumberStyles.Float, provider ?? CultureInfo.InvariantCulture, out double parsedValue))
        {
            result = new Celcius(parsedValue);
            return true;
        }

        result = default;
        return false;
    }

    // Not very usefull but mandatory from ITemperature implmendation.
    public Celcius ToCelsius() => new(Value);

    // Converts to Fahrenheit
    public Fahrenheit ToFahrenheit() => new((Value * 9) / 5 + 32);
}