using System;
using Xunit;

namespace ProjectReferencesRuler.Rules.References
{
    public class LiteralReferenceRuleBuilderTest
    {
        [Fact]
        public void BuildRule_KindNotSet_ThrowsInvalidOperationException()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .WithPrivateAssets(setToAll: true)
                .Version("12.0.0");

            Assert.Throws<InvalidOperationException>(() => ruleBuilder.BuildRule());
        }

        [Fact]
        public void BuildRule_AllFieldsSpecified_ReturnsCorrectRule()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .Kind(RuleKind.Forbidden)
                .WithPrivateAssets(setToAll: true)
                .Version("12.0.0");

            var rule = ruleBuilder.BuildRule();

            Assert.Equal("ProjectA", rule.PatternFrom);
            Assert.Equal("ProjectB", rule.PatternTo);
            Assert.Equal("A cool rule I made", rule.Description);
            Assert.Equal(RuleKind.Forbidden, rule.Kind);
            Assert.Equal(true, rule.IsPrivateAssetsAllSet);
            Assert.Equal("12.0.0", rule.Version);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingAllowed_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .Allowed();

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.Allowed, rule.Kind);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingForbidden_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .Forbidden();

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.Forbidden, rule.Kind);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingExplicitlyForbidden_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .ExplicitlyForbidden();

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.ExplicitlyForbidden, rule.Kind);
        }

        [Fact]
        public void BuildRule_PrivateAssetsAndVersionNotSpecified_ReturnsRuleWithThoseAsNull()
        {
            var ruleBuilder = ReferenceRule.Create("A cool rule I made")
                .From("ProjectA")
                .To("ProjectB")
                .Kind(RuleKind.Forbidden);

            var rule = ruleBuilder.BuildRule();

            Assert.Null(rule.IsPrivateAssetsAllSet);
            Assert.Null(rule.Version);
        }
    }
}
