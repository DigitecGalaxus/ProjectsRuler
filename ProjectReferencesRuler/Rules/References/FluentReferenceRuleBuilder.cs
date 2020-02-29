using System;

namespace ProjectReferencesRuler.Rules.References
{
    public class FluentReferenceRuleBuilder
    {
        private readonly string _patternFrom;
        private string _patternTo;
        private RuleKind? _kind;
        private string _description;
        private bool? _isPrivateAssetsAllSet;
        private string _version;

        internal FluentReferenceRuleBuilder(string patternFrom)
        {
            _patternFrom = patternFrom;
        }

        public FluentReferenceRuleBuilder Referencing(
            string patternTo,
            string version = null,
            bool? withPrivateAssetsAll = null)
        {
            _patternTo = patternTo;
            _version = version;
            _isPrivateAssetsAllSet = withPrivateAssetsAll;
            return this;
        }

        public FluentReferenceRuleBuilder Is(RuleKind kind)
        {
            _kind = kind;
            return this;
        }

        public FluentReferenceRuleBuilder IsAllowed() => Is(RuleKind.Allowed);
        public FluentReferenceRuleBuilder IsForbidden() => Is(RuleKind.Forbidden);
        public FluentReferenceRuleBuilder IsExplicitlyForbidden() => Is(RuleKind.ExplicitlyForbidden);

        public FluentReferenceRuleBuilder Called(string description)
        {
            _description = description;
            return this;
        }

        public ReferenceRule BuildRule()
        {
            return new ReferenceRule(
                patternFrom: _patternFrom,
                patternTo: _patternTo,
                kind: _kind ?? throw new InvalidOperationException($"Please specify a {nameof(ReferenceRule.Kind)} using the method {nameof(Is)}"),
                description: _description,
                isPrivateAssetsAllSet: _isPrivateAssetsAllSet,
                version: _version);
        }
    }
}
