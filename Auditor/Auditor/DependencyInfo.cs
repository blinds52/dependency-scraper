namespace Auditor
{
    public class DependencyInfo
    {
        public string RepositoryName { get; private set; }
        public string PackageId { get; private set; }
        public string Version { get; private set; }
        public string AllowedVersions { get; private set; }

        public DependencyInfo(string repositoryName, string packageId, string version, string allowedVersions)
        {
            RepositoryName = repositoryName;
            PackageId = packageId;
            Version = version;
            AllowedVersions = allowedVersions;
        }
    }
}