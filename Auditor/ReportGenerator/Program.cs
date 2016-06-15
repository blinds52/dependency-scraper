using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = args[0];

            // Read in the CSV and create the master list of Redgate.*
            var packageRepository = PackageRepository.Create(filename);

            // Map repositories to products and create a product view
            Dictionary<string,List<string>> productToRepos = new Dictionary<string, List<string>>
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

            using (var f = new StreamWriter(File.OpenWrite("report.csv")))
            {
                f.WriteLine("{0},{1},{2},{3},{4}", "Product", "PackageID", "Current Version","Latest Version", "Difference");
                foreach (var product in productToRepos.Keys)
                {
                    foreach (var dependency in packageRepository.GetDependencies(productToRepos[product]).Distinct())
                    {
                        var latestVersion = packageRepository.GetMaxVersion(dependency);


                        Version currentVersion;
                        if (Version.TryParse(dependency.Version, out currentVersion))
                        {
                            if (currentVersion < latestVersion)
                            {
                                f.WriteLine("{0},{1},{2},{3},{4}", product, dependency.PackageID, currentVersion,
                                    latestVersion, VersionDifference(latestVersion,currentVersion));
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine("Warning: Failed to parse {0}", dependency.Version);
                        }
                    }
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
    }}
