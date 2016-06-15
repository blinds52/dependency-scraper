using System;
using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;

namespace ReportGenerator
{
    public class PackageRepository
    {
        private List<DependencyInfo> _dependencyInfos;

        public PackageRepository(List<DependencyInfo> dependencyInfos)
        {
            this._dependencyInfos = dependencyInfos;
        }

        public static PackageRepository Create(string filename)
        {
            CsvFileDescription x = new CsvFileDescription()
            {
                IgnoreUnknownColumns = true,
                FirstLineHasColumnNames = true
            };

            CsvContext cc = new CsvContext();

            var dependencyInfos = cc.Read<DependencyInfo>(filename, x).ToList();

            return new PackageRepository(dependencyInfos);
        }

        public IEnumerable<DependencyInfo> GetDependencies(List<string> productToRepo)
        {
            return _dependencyInfos.Where(x => productToRepo.Contains(x.RepoName));
        }

        public Version GetMaxVersion(DependencyInfo dependency)
        {
            var maxVersion =
                _dependencyInfos.Where(x => x.PackageID == dependency.PackageID).Max(dependencyInfo =>
                {
                    Version result;
                    if (Version.TryParse(dependencyInfo.Version, out result))
                    {
                        return result;
                    }
                    else
                    {
                        return new Version(0,0,0);
                    }
                });
            return maxVersion;
        }
    }
}