using CommandLine;
using System;
using System.Collections.Generic;
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

            var reports = new List<string>();

            var lastReportPosition = 0;

            while (lastReportPosition != -1)
            {
                var currentReportPosition = readText.IndexOf("Nmap scan report for", lastReportPosition+1, StringComparison.Ordinal);

                if (currentReportPosition != -1)
                    reports.Add(readText.Substring(lastReportPosition, currentReportPosition - lastReportPosition));

                lastReportPosition = currentReportPosition;
            }

            // Remove "Starting Nmap" string
            if (reports.Count > 0)
                reports.RemoveAt(0);

            Console.WriteLine($@"Finded {reports.Count} reports");
        }
    }
}
