using System;

namespace ProjectReferencesRuler.Rules.References
{
    public class LiteralReferenceRuleBuilder
    {
        private readonly string Description;
        private string PatternFrom;
        private string PatternTo;
        private RuleKind? RuleKind;
        private bool? IsPrivateAssetsAllSet;
        private string ReferenceVersion;

        internal LiteralReferenceRuleBuilder(string description)
        {
            Description = description;
        }

        public LiteralReferenceRuleBuilder From(string patternFrom)
        {
            PatternFrom = patternFrom;
            return this;
        }

        public LiteralReferenceRuleBuilder To(string patternTo)
        {
            PatternTo = patternTo;
            return this;
        }

        public LiteralReferenceRuleBuilder Kind(RuleKind kind)
        {
            RuleKind = kind;
            return this;
        }

        public LiteralReferenceRuleBuilder Allowed() => Kind(Rules.RuleKind.Allowed);
        public LiteralReferenceRuleBuilder Forbidden() => Kind(Rules.RuleKind.Forbidden);
        public LiteralReferenceRuleBuilder ExplicitlyForbidden() => Kind(Rules.RuleKind.ExplicitlyForbidden);

        public LiteralReferenceRuleBuilder Version(string referenceVersion)
        {
            ReferenceVersion = referenceVersion;

            return this;
        }

        public LiteralReferenceRuleBuilder WithPrivateAssets(bool setToAll)
        {
            IsPrivateAssetsAllSet = setToAll;

            return this;
        }

        public ReferenceRule BuildRule()
        {
            return new ReferenceRule(
                patternFrom: PatternFrom,
                patternTo: PatternTo,
                kind: RuleKind ?? throw new InvalidOperationException($"Please specify a {nameof(ReferenceRule.Kind)} using the method {nameof(Kind)}"),
                description: Description,
                isPrivateAssetsAllSet: IsPrivateAssetsAllSet,
                version: ReferenceVersion);
        }
    }
}
