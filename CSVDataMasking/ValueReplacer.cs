using System;
using System.Collections.Generic;

namespace CSVDataMasking;

public class ValueReplacer
{
    private readonly Func<string, object, object> onNewValueNeeded;
    private readonly Dictionary<int, object> cache = new Dictionary<int, object>();

    public ValueReplacer(Func<string, object, object> onNewValueNeeded)
    {
        this.onNewValueNeeded = onNewValueNeeded;
    }

    public object GetNewValue(string columnName, object oldValue)
    {
        var hash = Hash(columnName, oldValue);
        if (cache.TryGetValue(hash, out var newValue))
        {
            return newValue;
        }
      
        newValue = onNewValueNeeded(columnName, oldValue);
        cache[hash] = newValue;
        return newValue;
    }

    private static int Hash(string columnName, object oldValue)
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = (int)2166136261;
            // Suitable nullity checks etc, to be added here.
            hash = (hash * 16777619) ^ oldValue.GetHashCode();
            hash = (hash * 16777619) ^ columnName.GetHashCode();
            return hash;
        }
    }
}