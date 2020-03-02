using System;

namespace ProjectReferencesRuler.Rules.References
{
    public class ReferenceRuleBuilder
    {
        private readonly string _patternFrom;
        private string _patternTo;
        private RuleKind? _kind;
        private string _description;
        private bool? _isPrivateAssetsAllSet;
        private string _version;

        internal ReferenceRuleBuilder(string patternFrom)
        {
            _patternFrom = patternFrom;
        }

        public ReferenceRuleBuilder Referencing(
            string patternTo,
            string version = null,
            bool? withPrivateAssetsAll = null)
        {
            _patternTo = patternTo;
            _version = version;
            _isPrivateAssetsAllSet = withPrivateAssetsAll;
            return this;
        }

        private ReferenceRuleBuilder Is(RuleKind kind)
        {
            _kind = kind;
            return this;
        }

        public ReferenceRuleBuilder IsAllowed() => Is(RuleKind.Allowed);
        public ReferenceRuleBuilder IsForbidden() => Is(RuleKind.Forbidden);
        public ReferenceRuleBuilder IsExplicitlyForbidden() => Is(RuleKind.ExplicitlyForbidden);

        public ReferenceRuleBuilder Because(string description)
        {
            _description = description;
            return this;
        }

        public ReferenceRule BuildRule()
        {
            return new ReferenceRule(
                patternFrom: _patternFrom,
                patternTo: _patternTo,
                kind: _kind ?? throw new InvalidOperationException($"Please specify a {nameof(ReferenceRule.Kind)} using e.g. {nameof(IsForbidden)}"),
                description: _description,
                isPrivateAssetsAllSet: _isPrivateAssetsAllSet,
                version: _version);
        }
    }
}
