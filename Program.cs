using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            foreach (var report in reports)
            {
                Regex regexIpAdd = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}");
                MatchCollection ipAddMatches = regexIpAdd.Matches(report);
                if (ipAddMatches.Count > 0)
                {
                    var directoryReport = Path.GetDirectoryName(filePath);
                    var reportFileName = $@"{ipAddMatches.First().Value.Replace('.','-')}.txt";

                    if (string.IsNullOrEmpty(directoryReport) || string.IsNullOrEmpty(reportFileName))
                    {
                        Console.WriteLine(@$"Error while processing {reportFileName} report: directory or file not found");
                        continue;
                    }
                    
                    var reportFilePath = Path.Combine(directoryReport, reportFileName);

                    Console.Write(@$"Writing {reportFileName} report... ");

                    if (File.Exists(reportFilePath))
                    {
                        Console.WriteLine();
                        Console.WriteLine(@$"File {reportFilePath} already exists. Rewrite? Y/N");
                        var answer = Console.ReadKey();
                        Console.WriteLine();
                        if (answer.Key != ConsoleKey.Y)
                        {
                            Console.WriteLine(@$"File {reportFilePath} skiped!");
                            continue;
                        }
                    }

                    using FileStream fs = File.Create(reportFilePath);
                    byte[] info = new UTF8Encoding(true).GetBytes(report);
                    fs.Write(info, 0, info.Length);

                    Console.WriteLine(@$"Done!");
                }
                else
                {
                    Console.WriteLine("Not finded report IP");
                }
            }
        }
    }
}