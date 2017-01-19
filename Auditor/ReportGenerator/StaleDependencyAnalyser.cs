using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportGenerator
{
    internal class StaleDependencyAnalyser
    {
        private readonly PackageRepository _packageRepository;
        private readonly Dictionary<string, List<string>> _productToRepos;

        public StaleDependencyAnalyser(PackageRepository packageRepository, Dictionary<string, List<string>> productToRepos)
        {
            _packageRepository = packageRepository;
            _productToRepos = productToRepos;
        }

        public IEnumerable<StaleDependency> StaleDependencies()
        {
            foreach (var product in _productToRepos.Keys)
            {
                foreach (var dependency in _packageRepository.GetDependencies(_productToRepos[product]).Distinct())
                {
                    var latestVersion = _packageRepository.GetMaxVersion(dependency);

                    Version currentVersion;
                    if (Version.TryParse(dependency.Version, out currentVersion))
                    {
                        if (currentVersion < latestVersion)
                        {
                            yield return new StaleDependency(product, dependency.PackageID, currentVersion, latestVersion);

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
}