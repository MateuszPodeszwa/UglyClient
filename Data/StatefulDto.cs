using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Data;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]

public abstract class StatefulDto<TObject> where TObject : StatefulDto<TObject>
{
    public bool IsModified { get; private set; }
    public required Func<Task<ApiResult>>? SaveAction { get; init; }
    
    private readonly Dictionary<string, object?> _originalValues = new();

    protected bool SetProperty<TField>(ref TField oldObj, TField newObj, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<TField>.Default.Equals(oldObj, newObj))
        {
            return false; // Value hasn't changed
        }

        // First time value is set/created doesn't matter.
        IsModified = !_originalValues.TryAdd(propertyName, newObj);
        oldObj = newObj;
        return true;
    }
    
    public async Task<ApiResult> SaveOnChangesAsync()
    {
        if (!IsModified)
        {
            return ApiResult.Success();
        }

        if (SaveAction is null)
        {
            return (ApiResult) Error.CustomHttpError(
                566,
                $"SaveAction was not configured for {typeof(TObject).Name}.");
        }
        
        ApiResult response = await SaveAction();
        
        if (response.IsSuccess) AcceptChanges();
        else Revert(); 
        
        return ApiResult.Success();
    }

    private void Revert() => throw new NotImplementedException("Revert() is not yet implemented");

    private void AcceptChanges()
    {
        IsModified = false;
        _originalValues.Clear();
    }
}