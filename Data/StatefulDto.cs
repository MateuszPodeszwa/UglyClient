using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;

namespace BeautifulClient.Data;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]

public abstract class StatefulDto<TObject> where TObject : StatefulDto<TObject>
{
    public bool IsModified { get; private set; }
    public required Func<TObject, Task<ApiResult>>? SaveAction { get; init; }
    
    private readonly Dictionary<string, object?> _originalValues = new();

    protected TField SetProperty<TField>(ref TField oldObj, TField newObj, [CallerMemberName] string propertyName = "")
    {
        bool isFirstTime = _originalValues.TryAdd(propertyName, newObj);

        // Exit early if the state hasn't changed.
        if (EqualityComparer<TField>.Default.Equals(oldObj, newObj))
        {
            return oldObj; 
        }

        // Mark as modified ONLY if this is the following mutation.
        if (!isFirstTime)
        {
            IsModified = true;
        }
        
        oldObj = newObj;
        return newObj;
    }
    
    // TODO: Consider extension method.
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
        
        ApiResult response = await SaveAction((TObject)this);
        
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