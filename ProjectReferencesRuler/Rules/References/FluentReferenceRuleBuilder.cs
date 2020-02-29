using System;

namespace ProjectReferencesRuler.Rules.References
{
    public class FluentReferenceRuleBuilder
    {
        private readonly string PatternFrom;
        private string PatternTo;
        private RuleKind? RuleKind;
        private string Description;
        private bool? IsPrivateAssetsAllSet;
        private string ReferenceVersion;

        internal FluentReferenceRuleBuilder(string patternFrom)
        {
            PatternFrom = patternFrom;
        }

        public FluentReferenceRuleBuilder Referencing(
            string patternTo,
            string version = null,
            bool? withPrivateAssetsAll = null)
        {
            PatternTo = patternTo;
            ReferenceVersion = version;
            IsPrivateAssetsAllSet = withPrivateAssetsAll;
            return this;
        }

        public FluentReferenceRuleBuilder Is(RuleKind kind)
        {
            RuleKind = kind;
            return this;
        }

        public FluentReferenceRuleBuilder IsAllowed() => Is(Rules.RuleKind.Allowed);
        public FluentReferenceRuleBuilder IsForbidden() => Is(Rules.RuleKind.Forbidden);
        public FluentReferenceRuleBuilder IsExplicitlyForbidden() => Is(Rules.RuleKind.ExplicitlyForbidden);

        public FluentReferenceRuleBuilder Called(string description)
        {
            Description = description;
            return this;
        }

        public ReferenceRule BuildRule()
        {
            return new ReferenceRule(
                patternFrom: PatternFrom,
                patternTo: PatternTo,
                kind: RuleKind ?? throw new InvalidOperationException($"Please specify a {nameof(ReferenceRule.Kind)} using the method {nameof(Is)}"),
                description: Description,
                isPrivateAssetsAllSet: IsPrivateAssetsAllSet,
                version: ReferenceVersion);
        }
    }
}
