using System;
using System.Collections.Generic;
using System.IO;

namespace ReportGenerator
{
    class Program
    {

        // TODO read in this from a file!
        private static Dictionary<string, List<string>> ProductsToRepos()
        {            
            var productToRepos = new Dictionary<string, List<string>>
            {
                { "SQL Prompt", new List<string> { "red-gate/SQLPrompt"} },
                { "SQL Compare", new List<string> { "red-gate/SQLCompareEngine", "red-gate/SQLCompareUIs" } },
                { "SQL Source Control", new List<string> {"red-gate/SQLSourceControl" } },
                { "DLM Automation", new List<string> { "red-gate/DLMAutomationShared", "red-gate/sqlrelease", "red-gate/sqlci" } },
                { "SQL Monitor", new List<string> {"red-gate/SQLMonitor" } },
                { "ReadyRoll", new List<string> { "red-gate/readyroll" }},
                { "SQL Clone", new List<string> { "red-gate/sqlclone" }},
                { "SQL Data Generator", new List<string> { "red-gate/sqldatagenerator" }},
                { "SQL Test", new List<string> { "red-gate/sqltest" }},
                { "SQL Doc", new List<string> { "red-gate/sqldoc" }},
                { "SQL Dependency Tracker", new List<string> { "red-gate/sqldependencytracker" }},
                { "SQL Backup", new List<string> { "red-gate/sqlbackup" }},
                { "SQL Index Manager", new List<string> { "red-gate/sqlindexmanager" }},
                { "DLM Dashboard", new List<string> { "red-gate/sqllighthouse" }},
                { "SQL Multiscript", new List<string> { "red-gate/sqlmultiscript" }},
                { "SQL Search", new List<string> { "red-gate/sqlsearch" }}
            };

            return productToRepos;
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Out.WriteLine("Usage: <list of dependencies>  <outputFile");
                return;
            }

            var packageInputFile = args[0];
            var outputFile = args[1];

            var staleDependencyAnalyser = new StaleDependencyAnalyser(PackageRepository.Create(packageInputFile), ProductsToRepos());

            using (var output = new StreamWriter(File.OpenWrite(outputFile)))
            {
                output.WriteLine("{0},{1},{2},{3},{4}", "Product", "PackageID", "Current Version","Latest Version", "Difference");
                foreach (var x in staleDependencyAnalyser.StaleDependencies())
                {
                    output.WriteLine("{0},{1},{2},{3},{4}", x.Product, x.PackageId, x.CurrentVersion,x.LatestVersion, VersionDifference(x.LatestVersion, x.CurrentVersion));
                }
            }
        }

        public static string VersionDifference(Version x, Version y)
        {
            var d1 = x.Major - y.Major;
            var d2 = x.Minor - y.Minor;
            var d3 = x.Build - y.Build;
            var d4 = x.Revision - y.Revision;

            if (d1 > 0) return "Major " + d1;
            if (d2 > 0) return "Minor " + d2;
            if (d3 > 0) return "Third " + d3;
            if (d4 > 0) return "Build " + d4;

            return "";
        }
    }
}
