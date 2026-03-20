using BeautifulClient.Extensions;

namespace BeautifulClient.Data;

public class StatefulDto
{
    public bool IsModified { get; private set; }

    protected bool SetProperty<TValue>(ref TValue oldObj, TValue newObj)
    {
        if (EqualityComparer<TValue>.Default.Equals(oldObj, newObj))
        {
            return false; // Value hasn't changed
        }
        
        oldObj = newObj;
        IsModified = true;
        return true;
    }
    
    public void AcceptChanges()
    {
        IsModified = false;
    }
}