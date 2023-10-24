using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Cache2Db;

public class Cache
{
    private readonly IDictionary<string, string> _hitList = new Dictionary<string, string>();

    private readonly IDictionary<string, string> _wordList = new Dictionary<string, string>();

    private int _counter;

    /// <summary>
    ///     Computes the MD5 hash of the given string payload.
    /// </summary>
    /// <param name="payload">The string input for which the MD5 hash needs to be computed.</param>
    /// <returns>A byte array representing the MD5 hash of the input string.</returns>
    /// <remarks>
    ///     This method uses the MD5 cryptographic hash function. MD5 is considered cryptographically broken and unsuitable for
    ///     further use.
    ///     It's vulnerable to hash collisions. If you're using this for cryptographic purposes, consider using a more secure
    ///     hashing algorithm like SHA-256.
    ///     If you're using it for non-cryptographic purposes (like checksums), then it might be okay, but always be aware of
    ///     its limitations.
    /// </remarks>
    /// <example>
    ///     <code>
    /// string input = "Hello World";
    /// byte[] hash = MD5HashFromString(input);
    /// </code>
    /// </example>
    private static byte[] MD5HashFromString(string payload)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(payload);
        var hashBytes = md5.ComputeHash(inputBytes);
        return hashBytes;
    }

    public void LoadWordList(string wordList, string hitList)
    {
        if (!File.Exists(wordList))
            throw new FileNotFoundException("Wordlist not found", wordList);
        
        var lines = File.ReadLines(wordList).ToList();
        var random = new Random();

        // Fisher-Yates shuffle
        for (var i = lines.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (lines[i], lines[j]) = (lines[j], lines[i]);
        }

        foreach (var line in lines) _wordList.Add(Convert.ToBase64String(MD5HashFromString(line)), line);
        
        if (!File.Exists(hitList))
            return;
        
        ReadDictionaryFromFile(hitList);
    }

    private string Lookup(string hashString)
    {
        if (_hitList.TryGetValue(hashString, out var value))
            return _wordList[value];

        var (key, s) = _wordList.ElementAt(_counter++);

        _hitList.Add(hashString, key);

        return s;
    }

    public string GetReplacement(string original)
    {
        if (string.IsNullOrEmpty(original))
            return original;

        if (_wordList.Count == 0)
            throw new ApplicationException("Wordlist not initialized");

        return Lookup(Convert.ToBase64String(MD5HashFromString(original)));
    }

    public void SaveDictionaryToFile(string filePath)
    {
        var json = JsonSerializer.Serialize(_hitList);
        File.WriteAllText(filePath, json);
    }

    public void ReadDictionaryFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _hitList.Clear();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            foreach (var item in data) _hitList.Add(item.Key, item.Value);
        }
    }
}