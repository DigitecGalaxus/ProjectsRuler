using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProjectReferencesRuler.Rules.References
{
    /// <summary>
    /// Iterates through all the project references and challenges them against the given set of rules. If a rule is
    /// broken it generates a complaint. There can be forbidding and allowing rules. The forbidding rules are the main
    /// ones whereas the allowing ones are exceptions from the main rules and can override them.
    /// </summary>
    public class ReferencesRuler : IReferencesRuler
    {
        private readonly IPatternParser _patternParser;
        private readonly IReadOnlyList<ReferenceRule> _forbiddingRules;
        private readonly IReadOnlyList<ReferenceRule> _allowingRules;
        private readonly IReadOnlyList<ReferenceRule> _explicitlyForbiddenRules;

        public ReferencesRuler(IPatternParser patternParser, IReadOnlyList<ReferenceRule> rules)
        {
            _patternParser = patternParser;
            _forbiddingRules = rules.Where(r => r.Kind == RuleKind.Forbidden).ToList();
            _allowingRules = rules.Where(r => r.Kind == RuleKind.Allowed).ToList();
            _explicitlyForbiddenRules = rules.Where(r => r.Kind == RuleKind.ExplicitlyForbidden).ToList();
        }

        public string GiveMeComplaints(Reference reference)
        {
            var explicitlyForbiddenRules = GetExplicitlyForbiddenRules(reference).ToList();

            var generallyForbiddenRules = GetGenerallyForbiddenRules(reference);
            var areGeneralRolesOverriddenByExplicitlyAllowedRule = GetExplicitlyAllowedRules(reference).Any();
            if (!explicitlyForbiddenRules.Any() && areGeneralRolesOverriddenByExplicitlyAllowedRule)
            {
                return string.Empty;
            }

            var violatedRules = areGeneralRolesOverriddenByExplicitlyAllowedRule
                ? explicitlyForbiddenRules
                : explicitlyForbiddenRules.Union(generallyForbiddenRules);

            var complaints = string.Join("\n", violatedRules.Select(r => r.Description));
            if (string.IsNullOrEmpty(complaints))
            {
                return string.Empty;
            }

            return $"Reference from {reference.From} to {reference.To} broke:\n{complaints}";
        }

        private IEnumerable<ReferenceRule> GetGenerallyForbiddenRules(Reference reference)
        {
            return GetMatchingRules(rules: _forbiddingRules, reference: reference, kind: RuleKind.Forbidden);
        }

        private IEnumerable<ReferenceRule> GetExplicitlyAllowedRules(Reference reference)
        {
            return GetMatchingRules(rules: _allowingRules, reference: reference, kind: RuleKind.Allowed);
        }

        private IEnumerable<ReferenceRule> GetExplicitlyForbiddenRules(Reference reference)
        {
            return GetMatchingRules(rules: _explicitlyForbiddenRules, reference: reference, kind: RuleKind.ExplicitlyForbidden);
        }

        private IEnumerable<ReferenceRule> GetMatchingRules(IReadOnlyList<ReferenceRule> rules, Reference reference, RuleKind kind)
        {
            foreach (var rule in rules)
            {
                if (Regex.IsMatch(reference.From, _patternParser.GetRegex(rule.PatternFrom))
                    && Regex.IsMatch(reference.To, _patternParser.GetRegex(rule.PatternTo))
                    && DoesPrivateAssetsRuleMatch(reference, rule)
                    && DoesVersionRuleMatch(reference, rule)
                    && rule.Kind == kind)
                {
                    yield return rule;
                }
            }
        }

        private static bool DoesPrivateAssetsRuleMatch(Reference reference,
            ReferenceRule rule)
        {
            // if the value is not set, it is neutral -> true
            if (!rule.IsPrivateAssetsAllSet.HasValue)
            {
                return true;
            }

            return rule.IsPrivateAssetsAllSet.Value == reference.IsPrivateAssetsAllSet;
        }

        private static bool DoesVersionRuleMatch(Reference reference,
            ReferenceRule rule)
        {
            // if the value is not set, it is neutral -> true
            if (rule.Version == null)
            {
                return true;
            }

            return rule.Version == reference.VersionOrNull;
        }
    }
}
