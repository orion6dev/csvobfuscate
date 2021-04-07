//
//  COPYRIGHT (C) 2021 Orion6 Software Engineering,
//                     all rights reserved.
//
//

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

#endregion

var separator = ',';

var historyFileName = "history.txt";
if (!File.Exists(args[0]))
{
    throw new Exception("Input file not found");
}

if (File.Exists(args[1]))
{
    throw new Exception("Output file exits");
}

// Get an enumeration of all strings in the file.
var payload = File.ReadLines(
    args[0]
  , Encoding.Latin1);

// Determine the headers.
var Headers =
    payload
        .First()
        .Split(separator);

// Read the headers of the columns to be masked.
var MaskHeaders =
    File
        .ReadLines("mask-header.txt");

// Determine the column number of the headers
var columnNumbers = Headers
    .Select((_, index) => index)
    .Where(index => MaskHeaders.Contains(Headers[index]));

// Create a dictionary where we store the original value and the masked value
// combination.
var maskedValues = new Dictionary<string, string>();

// Read historical values so we keep a consistent view of the
// test data.

if (File.Exists(historyFileName))
{
    maskedValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(
        File.ReadAllText(historyFileName));
}

foreach (var columnNumber in columnNumbers)
{
    // Determine all distinct values in the column

    var ColumnValues =
        payload
            .Skip(1) // skip header record
            .Select(item => item.Split(separator)) // split record in columns
            .Select(columnValue => columnValue[columnNumber]) // select the column value
            .Distinct(); // remove duplicates

    var counter = maskedValues.Count;

    // For all found distinct column values, we generate a masked value
    foreach (var columnValue in ColumnValues)
        if (!maskedValues.ContainsKey(columnValue))
        {
            maskedValues.Add(
                columnValue // The original column value
              , $"{Headers[columnNumber]} {counter++:0000}"); // the masked value
        }
}

// As of here, the maskedValues dictionary contains the original column content (key)
// and the masked value (value).

// Now replace all original content with the masked value and write out to file.
File.WriteAllText(
    args[1]
  , maskedValues.Aggregate(
        File.ReadAllText(
            args[0]
          , Encoding.Latin1)
      , (current, kv) => current.Replace(
            kv.Key
          , kv.Value)));

// Write out the obfuscated values for future reference.

File.WriteAllText(
    historyFileName
  , JsonConvert.SerializeObject(
        maskedValues
      , (Formatting) System.Xml.Formatting.Indented));
