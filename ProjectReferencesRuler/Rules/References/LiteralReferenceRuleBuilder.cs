using System;

namespace ProjectReferencesRuler.Rules.References
{
    public class LiteralReferenceRuleBuilder
    {
        private readonly string _description;
        private string _patternFrom;
        private string _patternTo;
        private RuleKind? _kind;
        private bool? _isPrivateAssetsAllSet;
        private string _version;

        internal LiteralReferenceRuleBuilder(string description)
        {
            _description = description;
        }

        public LiteralReferenceRuleBuilder From(string patternFrom)
        {
            _patternFrom = patternFrom;
            return this;
        }

        public LiteralReferenceRuleBuilder To(string patternTo)
        {
            _patternTo = patternTo;
            return this;
        }

        public LiteralReferenceRuleBuilder Kind(RuleKind kind)
        {
            _kind = kind;
            return this;
        }

        public LiteralReferenceRuleBuilder Allowed() => Kind(RuleKind.Allowed);
        public LiteralReferenceRuleBuilder Forbidden() => Kind(RuleKind.Forbidden);
        public LiteralReferenceRuleBuilder ExplicitlyForbidden() => Kind(RuleKind.ExplicitlyForbidden);

        public LiteralReferenceRuleBuilder Version(string referenceVersion)
        {
            _version = referenceVersion;
            return this;
        }

        public LiteralReferenceRuleBuilder WithPrivateAssets(bool setToAll)
        {
            _isPrivateAssetsAllSet = setToAll;
            return this;
        }

        public ReferenceRule BuildRule()
        {
            return new ReferenceRule(
                patternFrom: _patternFrom,
                patternTo: _patternTo,
                kind: _kind ?? throw new InvalidOperationException($"Please specify a {nameof(ReferenceRule.Kind)} using the method {nameof(Kind)}"),
                description: _description,
                isPrivateAssetsAllSet: _isPrivateAssetsAllSet,
                version: _version);
        }
    }
}
