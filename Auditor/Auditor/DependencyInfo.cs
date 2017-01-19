namespace Auditor
{
    public class DependencyInfo
    {
        public string ItemPath { get; private set; }
        public string RepositoryName { get; private set; }
        public string PackageId { get; private set; }
        public string Version { get; private set; }
        public string AllowedVersions { get; private set; }

        public DependencyInfo(string repositoryName, string packageId, string version, string allowedVersions, string itemPath)
        {
            ItemPath = itemPath;
            RepositoryName = repositoryName;
            PackageId = packageId;
            Version = version;
            AllowedVersions = allowedVersions;
        }
    }
}