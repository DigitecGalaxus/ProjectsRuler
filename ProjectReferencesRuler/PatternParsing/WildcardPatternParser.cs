using System.Text.RegularExpressions;

namespace ProjectReferencesRuler
{
    public class WildcardPatternParser : IPatternParser
    {
        public string GetRegex(string plainWildcardPattern)
        {
            return $"^{Regex.Escape(plainWildcardPattern).Replace("\\*", ".*").Replace("\\?", ".")}$";
        }
    }
}