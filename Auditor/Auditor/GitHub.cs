using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Octokit;

namespace Auditor
{
    public class GitHub
    {
        private readonly GitHubClient _gitHubClient;

        private static readonly int ResultsPerPage = 100;

        private RateLimit _rateLimit;

        public GitHub(GitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        public IEnumerable<DependencyInfo> EnumerateDependenciesForOrganization(string organization)
        {
            // We have to search for repositories first since there's a limit of 1000 results return
            return Repositories(organization).SelectMany(repo => FindRepoDependencies(repo).ToList());
        }

        private IEnumerable<Repository> Repositories(string organization)
        {
            CheckRateLimit();
            return _gitHubClient.Repository.GetAllForOrg(organization).Result;
        }

        private IEnumerable<DependencyInfo> FindRepoDependencies(Repository repo)
        {
            int currentPage = 0;
            int totalResults;
            do
            {
                var result = FindPackagesConfigInRepo(repo, currentPage++);
                totalResults = result.TotalCount;
                if (result.IncompleteResults)
                {
                    Console.Error.WriteLine("Failed to retrieve complete search results, continuing anyway since something is better than nothing.");
                }

                Console.WriteLine("Processing: {0}, found {1} results",repo.FullName, totalResults );

                foreach (var dependencyInfo in result.Items.Select(item => new {item, content = GetBlob(item)}).SelectMany(t => ParsePackageDependencies(t.content, t.item)))
                {
                    yield return dependencyInfo;
                }
            } while (currentPage * ResultsPerPage <= totalResults);
        }

        private static IEnumerable<DependencyInfo> ParsePackageDependencies(string content, SearchCode item)
        {
            XDocument xdoc = null;
            try
            {
                xdoc = XDocument.Parse(content);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Failed to parse packages.config at {0} because of {1}", item.Path, ex);   
                yield break;
            }

            foreach (var package in xdoc.Descendants(XName.Get("package")))
            {
                var packageId = package.Attributes(XName.Get("id")).FirstOrDefault()?.Value;
                var version = package.Attributes(XName.Get("version")).FirstOrDefault()?.Value;
                var allowedVersions = package.Attributes(XName.Get("allowedVersions")).FirstOrDefault()?.Value;

                yield return new DependencyInfo(item.Repository.FullName, packageId, version, allowedVersions);
            }
        }

        private string GetBlob(SearchCode item)
        {
            CheckRateLimit();

            var blob = _gitHubClient.Git.Blob.Get(item.Repository.Owner.Login, item.Repository.Name, item.Sha).Result;

            return Encoding.UTF8.GetString(Convert.FromBase64String(blob.Content)).Trim('\uFEFF'); // Trim BOM
        }

        private SearchCodeResult FindPackagesConfigInRepo(Repository repo, int page)
        {
            var repositoryCollection = new RepositoryCollection {repo.Owner.Login + "/" + repo.Name};

            // Search API is rate limited by something unknown to OctoKit.net (see https://developer.github.com/v3/rate_limit/)
            Thread.Sleep(2000);

            return _gitHubClient.Search.SearchCode(new SearchCodeRequest("filename:packages.config")
            {
                PerPage = ResultsPerPage,
                Page = page,
                Repos = repositoryCollection
            }).Result;
        }

        private void CheckRateLimit()
        {
            
            var lastRateLimit = _gitHubClient.GetLastApiInfo()?.RateLimit;
            if (lastRateLimit != null)
            {
                _rateLimit = lastRateLimit;
            }
            if (_rateLimit != null && _rateLimit.Remaining <= 1)
            {
                var rateLimitResetTime = UnixTimeStampToDateTime(_rateLimit.ResetAsUtcEpochSeconds);
                while (DateTime.UtcNow < rateLimitResetTime)
                {
                    TimeSpan s = DateTime.Now - rateLimitResetTime;
                    Console.WriteLine("Rate limit approaching. I need to calm for {0} seconds", s.TotalSeconds);
                    Thread.Sleep(s);
                }
            }
        }

        // http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}