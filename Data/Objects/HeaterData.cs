using BeautifulClient.Utilities.Pipelines;

namespace BeautifulClient.Data.Objects;

public class HeaterData(IObjectSetterPipeline pipeline) : 
    StatefulDto<HeaterData>,    // Defines history and states for the data object 
    IData                       // Establishes the "base" or minimal shape of each DTO
{
    /// <inheritdoc/>>
    public int Id { get; init; }
    /// <inheritdoc/>>
    public string? RawJson { get; set; }

    public int Level
    {
        get;
        set => pipeline.Execute((() => SetProperty(ref field, value)));
    }
    /// <inheritdoc/>>
    public override string ToString()
    {
        // DtoActions is relying on this override.
        return RawJson ?? string.Empty;
    }
}