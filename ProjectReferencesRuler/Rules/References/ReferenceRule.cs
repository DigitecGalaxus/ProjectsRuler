namespace ProjectReferencesRuler.Rules.References
{
    public class ReferenceRule
    {
        public readonly string PatternFrom;
        public readonly string PatternTo;
        public readonly RuleKind Kind;
        public readonly string Description;
        public readonly bool? IsPrivateAssetsAllSet;
        public readonly string Version;

        public ReferenceRule(string patternFrom, string patternTo, RuleKind kind, string description, bool? isPrivateAssetsAllSet = null, string version = null)
        {
            PatternFrom = patternFrom;
            PatternTo = patternTo;
            Kind = kind;
            Description = description;
            IsPrivateAssetsAllSet = isPrivateAssetsAllSet;
            Version = version;
        }

        public static LiteralReferenceRuleBuilder Create(string description)
        {
            return new LiteralReferenceRuleBuilder(description);
        }

        public static FluentReferenceRuleBuilder For(string patternFrom)
        {
            return new FluentReferenceRuleBuilder(patternFrom);
        }
    }
}
