using System.Collections.Generic;
using CommandLine;

namespace CSVDataMasking.DataMask;

public class DataMaskOptions
{
    [Option(
        "columns-to-mask"
        , Required = true
        , HelpText = "Columns which values are masked."
    )]
    public IEnumerable<string> ColumnsToReplace { get; set; }

    [Option(
        "word-list"
        , Required = true
        , Default = "wordlist.10000.txt"
        , HelpText = "List with words to use as replacement."
    )]
    public string Wordlist { get; set; }

    [Option(
        "hit-list"
        , Required = true
        , Default = "hitlist.json"
        , HelpText = "hit hashes."
    )]
    public string Hitlist { get; set; }

    [Option(
        'i'
        , "input-file"
        , Required = true
        , HelpText = "CSV file to process."
    )]
    public string Inputfile { get; set; }

    [Option(
        'o'
        , "output-file"
        , Required = true
        , HelpText = "CSV file target."
    )]
    public string Outputfile { get; set; }

    [Option(
        'd'
        , "delimiter"
        , Required = false
        , Default = "|"
        , HelpText = "Delimiter."
    )]
    public string Delimiter { get; set; }
}