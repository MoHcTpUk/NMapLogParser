using CommandLine;
using System;
using System.IO;

namespace NMapLogParser
{
    class Program
    {
        public class Options
        {
            [Option('f', "file", Required = true, HelpText = "Set file to parse")]
            public string File { get; set; }
        }

        static void Main(string[] args)
        {
            string filePath = "";

            var parseResult = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    filePath = o.File;
                });

            if (parseResult.Tag == ParserResultType.NotParsed)
            {
                Environment.Exit(0);
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine(@$"File {filePath} not found");
                Environment.Exit(0);
            }

            string readText = File.ReadAllText(filePath);
            Console.WriteLine(readText);
        }
    }
}
