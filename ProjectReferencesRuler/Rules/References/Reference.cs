namespace ProjectReferencesRuler.Rules.References
{
    public class Reference
    {
        public readonly string From;
        public readonly string To;
        public readonly bool IsPrivateAssetsAllSet;
        public readonly string VersionOrNull;

        public Reference(
            string @from,
            string to,
            bool isPrivateAssetsAllSet,
            string versionOrNull)
        {
            From = @from;
            To = to;
            IsPrivateAssetsAllSet = isPrivateAssetsAllSet;
            VersionOrNull = versionOrNull;
        }
    }
}