using BeautifulClient.Data;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Services.Api;

/// <summary>
/// Defines the domain-specific contract between the application and the hardware API.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> To outline the explicit hardware capabilities supported by the API (e.g., fetching sensor data, toggling fans), entirely distinct from the underlying generic HTTP operations.</para>
/// <para><b>Strategy:</b> Promotes a thin-client architecture by encapsulating endpoint URIs and data mapping. This enforces the DRY principle and completely decouples the application logic from external API complexities, ensuring the system remains highly testable and maintainable.</para>
/// <para><b>Pattern:</b> Implements the Service (or Facade) pattern to provide a simplified, strongly-typed interface over the raw <see cref="IApiActions"/> network calls.</para>
/// </remarks>
public interface IHardwareApiService
{
    Task<ApiResult<SensorData>> GetSensorTemperatureAsync(int sensorId);
    Task SetHeaterLevelAsync(int heaterId, int level);
    Task SetFanStateAsync(int fanId, bool isOn);
}