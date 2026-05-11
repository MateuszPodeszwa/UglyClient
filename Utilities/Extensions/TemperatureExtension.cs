// ReSharper disable All

namespace BeautifulClient.Utilities.Extensions;

public static class TemperatureExtensions
{
    public static celc Celsius(this double value) => new celc(value);
    public static celc Celsius(this int value) => new celc(value);
    public static celc Celsius(this float value) => new celc(value);
    public static celc Round(this celc value, int precision = 2) => Math.Round(value.Value, precision).Celsius();
}