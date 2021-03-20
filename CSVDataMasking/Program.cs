using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVDataMasking
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.Combine(AppContext.BaseDirectory))
                                                    .AddJsonFile("appsettings.json", optional: true);
            Configuration = builder.Build();
            var inputFilePath = Configuration["Settings:InputFilePath"].ToString();
            var outputFilePath = Configuration["Settings:OutputFilePath"].ToString();
            var maskingColumnNames = Configuration["Settings:MaskingColumnNames"].ToString().Split(',');
            var outputfileName = Configuration["Settings:outputFileName"].ToString();

            // read a CSV
            // convert data to DataTable for easy Parsing
            var dt = ConvertCSVtoDataTable(inputFilePath);

            if (dt != null)
            {
                // Loop through all the columns of DataTable and Apply masking using the array
                foreach (DataColumn col in dt.Columns)
                {
                    if (Array.IndexOf(maskingColumnNames, col.ColumnName) > -1)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            row.SetField(col, $"{col.ColumnName} # {i}");
                        }
                    }
                }

                // save the resultant DataTable in a CSV
                ExportDatatableToCSV(dt, outputFilePath, outputfileName);
            }
        }


        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            try
            {
                DataTable dt = new DataTable();
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }

                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in trying to convert CSV to DataTable | Error: {ex.Message}");
                return new DataTable();
            }
        }

        public static void ExportDatatableToCSV(DataTable dt, string outputFilePath, string outputfileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText($"{outputFilePath}\\{outputfileName}", sb.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in trying to Export DataTable to CSV | Error: {ex.Message}");
            }
        }
    }
}
