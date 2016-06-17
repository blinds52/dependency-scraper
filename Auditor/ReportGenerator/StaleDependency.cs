using System;

namespace ReportGenerator
{
    public class StaleDependency
    {
        public StaleDependency(string product, string packageId, Version currentVersion, Version latestVersion)
        {
            Product = product;
            PackageId = packageId;
            CurrentVersion = currentVersion;
            LatestVersion = latestVersion;
        }

        public string Product { get; private set; }
        public string PackageId { get; private set; }
        public Version CurrentVersion { get; private set; }
        public Version LatestVersion { get; private set; }
    }
}