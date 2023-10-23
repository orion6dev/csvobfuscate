using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVDataMasking;

internal static class Program
{
    private static void Main(string[] args)
    {
        var inputFilePath = @"c:\temp\dump.csv";
        var outputFilePath = @"c:\temp\masked.csv";

        var i = File.OpenRead(inputFilePath);

        using var sr = new StreamReader(i, Encoding.UTF8, true);

        var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("de-de"))
        {
            Delimiter = "|",
            TrimOptions = TrimOptions.Trim,
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
            ShouldQuote = args => true
        };

        using var csvReader = new CsvReader(sr, csvConfiguration);
        csvReader.Read();
        csvReader.ReadHeader();

        using var sw = new StreamWriter(outputFilePath, false, new UTF8Encoding(true));

        var csvWriter = new CsvWriter(sw, csvConfiguration);

        // Write headers to the new CSV file.
        foreach (var header in csvReader.HeaderRecord)
        {
            csvWriter.WriteField(header);
        }

        csvWriter.NextRecord();

        // Read and modify records.
        while (csvReader.Read())
        {
            var record = csvReader.GetRecord<dynamic>() as IDictionary<string, object>;

            // Modify a specific column's value. Replace "ColumnName" with your actual column name.
                var oldValue = record["LongText"];
                record["LongText"] = oldValue + "..."; // Modify however you like.

            // Write the record.
            foreach (var value in record.Values)
            {
                csvWriter.WriteField(value);
            }

            csvWriter.NextRecord();
        }
    }
}