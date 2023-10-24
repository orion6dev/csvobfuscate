using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Cache2Db;
using CsvHelper;
using CsvHelper.Configuration;

namespace CSVDataMasking.DataMask;

public class DataMaskImplementation
{
    private static string PrependDateTimeToFilename(string filePath)
    {
        // Extract directory, filename without extension, and extension
        var directory = Path.GetDirectoryName(filePath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        var extension = Path.GetExtension(filePath);

        // Prepend the current date-time in yyyyMMddhhmmss format to the filename
        var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileNameWithoutExtension}{extension}";

        // Combine the directory and new filename to get the full path
        var newFilePath = Path.Combine(directory, newFileName);

        return newFilePath;
    }
    public static int Run(DataMaskOptions options)
    {

        var i = File.OpenRead(options.Inputfile);

        using var sr = new StreamReader(i, Encoding.UTF8, true);

        var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("de-de"))
        {
            Delimiter = options.Delimiter,
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
        
        cache.LoadWordList(options.Wordlist, options.Hitlist);
        
        using var sw = new StreamWriter(PrependDateTimeToFilename(options.Outputfile), false, new UTF8Encoding(true));
        var csvWriter = new CsvWriter(sw, csvConfiguration);

        // Write headers to the new CSV file.
        foreach (var header in csvReader.HeaderRecord) csvWriter.WriteField(header);

        csvWriter.NextRecord();

        while (csvReader.Read())
        {
            var record = csvReader.GetRecord<dynamic>() as IDictionary<string, object>;

            // Modify a specific column's value. Replace "ColumnName" with your actual column name.
            foreach (var columnName in options.ColumnsToReplace)
            {
                var oldValue = record[columnName];
                record[columnName] = cache.GetReplacement(oldValue as string ?? throw new InvalidOperationException());
            }

            // Write the record.
            foreach (var value in record.Values) csvWriter.WriteField(value);

            csvWriter.NextRecord();
        }
        
        cache.SaveDictionaryToFile(options.Hitlist);

        return 0;
    }
}