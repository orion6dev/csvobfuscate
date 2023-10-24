using CommandLine;
using CSVDataMasking.DataMask;

var result = 0;

var parsed = 
    Parser.Default.ParseArguments<DataMaskOptions>(args)
        .WithParsed(o => result = DataMaskImplementation.Run(o));

return result;