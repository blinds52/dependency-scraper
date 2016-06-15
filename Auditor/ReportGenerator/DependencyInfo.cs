using LINQtoCSV;

namespace ReportGenerator
{
    public class DependencyInfo
    {
        [CsvColumn(Name = "Repository Name", FieldIndex = 1)]
        public string RepoName { get; set; }

        [CsvColumn(Name = "Package Id", FieldIndex = 2)]
        public string PackageID { get; set; }

        [CsvColumn(Name = "Version", FieldIndex = 3)]
        public string Version { get; set; }

        protected bool Equals(DependencyInfo other)
        {
            return string.Equals(RepoName, other.RepoName) && string.Equals(PackageID, other.PackageID) && string.Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DependencyInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RepoName != null ? RepoName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PackageID != null ? PackageID.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(DependencyInfo left, DependencyInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DependencyInfo left, DependencyInfo right)
        {
            return !Equals(left, right);
        }
    }
}