namespace ProjectReferencesRuler
{
    internal static class PathNormalizer
    {
        /// <summary>
        /// Replaces \ with / so the same code works on both Windows and Linux.
        /// </summary>
        public static string CleanPath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
