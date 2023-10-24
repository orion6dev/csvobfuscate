using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CSVDataMasking;

public class ValueReplacer
{
    private static readonly string FileName = @"c:\temp\replaced.txt";
    private readonly Func<string, object, object> onNewValueNeeded;
    private Dictionary<int, object> cache;

    public ValueReplacer(Func<string, object, object> onNewValueNeeded)
    {
        this.onNewValueNeeded = onNewValueNeeded;
        cache = new Dictionary<int, object>();
    }

    public void LoadCacheFromFile()
    {
        if (File.Exists(FileName))
        {
            var contents = File.ReadAllText(FileName);
            cache = JsonSerializer.Deserialize<Dictionary<int, object>>(contents) ?? new Dictionary<int, object>();
        }
    }

    public object GetNewValue(string columnName, object oldValue)
    {
        var hash = Hash(columnName, oldValue);
        if (cache.TryGetValue(hash, out var newValue)) return newValue;

        newValue = onNewValueNeeded(columnName, oldValue);
        cache[hash] = newValue;
        return newValue;
    }

    public void SaveCacheToFile()
    {
        var contents = JsonSerializer.Serialize(cache);
        File.WriteAllText(FileName, contents);
    }

    private static int Hash(string columnName, object oldValue)
    {
        unchecked // Overflow is fine, just wrap
        {
            var hash = (int)2166136261;
            // Suitable nullity checks etc, to be added here.
            hash = (hash * 16777619) ^ oldValue.GetHashCode();
            hash = (hash * 16777619) ^ columnName.GetHashCode();
            return hash;
        }
    }
}