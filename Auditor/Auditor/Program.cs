using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NuGet;
using Octokit;
using Octokit.Internal;

namespace Auditor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: Auditor <organization> <accesstoken> <outputfile>");
                return;
            }

            var organization = args[0];
            var accessToken = args[1];
            var outputFile = args[2];

            var client = CreateGitHubClient(accessToken);

            try
            {
                using (var sw = new StreamWriter(File.OpenWrite(outputFile)))
                {
                    sw.WriteLine("Repository Name, Package Id, Version, Allowed Versions");
                    foreach (var dependency in client.EnumerateDependenciesForOrganization(organization))
                    {
                        sw.WriteLine("{0},{1},{2},{3}", dependency.RepositoryName, dependency.PackageId, dependency.Version, dependency.AllowedVersions);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Balls. {0}", ex);
            }
        }

        private static GitHub CreateGitHubClient(string accessToken)
        {
            var credentialStore = new InMemoryCredentialStore(new Credentials(accessToken));
            var gitHubClient = new GitHubClient(new ProductHeaderValue("info-scraper"), credentialStore);
            return new GitHub(gitHubClient);
        }
    }
}
