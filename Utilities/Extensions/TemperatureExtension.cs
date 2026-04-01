// ReSharper disable All

namespace BeautifulClient.Utilities.Extensions;

public static class TemperatureExtensions
{
    public static celc Celsius(this double value) => new celc(value);
    public static celc Celsius(this int value) => new celc(value);
}