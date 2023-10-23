namespace CSVDataMasking;

public class ObfuscateValue
{
    public static string Obfuscate(string columnName, string value)
    {
        if (columnName == "LongText")
            return value + "...";
        return value;
    }
}