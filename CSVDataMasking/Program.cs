using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Cache2Db;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVDataMasking;

internal static class Program
{
    private static void Main(string[] args)
    {
        var ColumnsToReplace = new[] {"Description1", "Description2", "MatchCode", "LongText"}; 
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

        var cache = new Cache();
        
        using var sw = new StreamWriter(outputFilePath, false, new UTF8Encoding(true));
        var csvWriter = new CsvWriter(sw, csvConfiguration);

        // Write headers to the new CSV file.
        foreach (var header in csvReader.HeaderRecord) csvWriter.WriteField(header);

        csvWriter.NextRecord();

        while (csvReader.Read())
        {
            var record = csvReader.GetRecord<dynamic>() as IDictionary<string, object>;

            // Modify a specific column's value. Replace "ColumnName" with your actual column name.
            foreach (var columnName in ColumnsToReplace)
            {
                var oldValue = record[columnName];
                record[columnName] = cache.GetReplacement(oldValue as string ?? throw new InvalidOperationException());
            }

            // Write the record.
            foreach (var value in record.Values) csvWriter.WriteField(value);

            csvWriter.NextRecord();
        }
    }
}