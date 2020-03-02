using System;
using Xunit;

namespace ProjectReferencesRuler.Rules.References
{
    public class ReferenceRuleBuilderTest
    {
        [Fact]
        public void BuildRule_KindNotSet_ThrowsInvalidOperationException()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB")
                .Because("A cool rule I made");

            Assert.Throws<InvalidOperationException>(() => ruleBuilder.BuildRule());
        }

        [Fact]
        public void BuildRule_AllFieldsSpecified_ReturnsCorrectRule()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB", version: "12.0.0", withPrivateAssetsAll: true)
                .IsForbidden()
                .Because("A cool rule I made");

            var rule = ruleBuilder.BuildRule();

            Assert.Equal("ProjectA", rule.PatternFrom);
            Assert.Equal("ProjectB", rule.PatternTo);
            Assert.Equal("A cool rule I made", rule.Description);
            Assert.Equal(RuleKind.Forbidden, rule.Kind);
            Assert.Equal(true, rule.IsPrivateAssetsAllSet);
            Assert.Equal("12.0.0", rule.Version);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingIsAllowed_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB", version: "12.0.0", withPrivateAssetsAll: true)
                .IsAllowed()
                .Because("A cool rule I made");

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.Allowed, rule.Kind);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingIsForbidden_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB", version: "12.0.0", withPrivateAssetsAll: true)
                .IsForbidden()
                .Because("A cool rule I made");

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.Forbidden, rule.Kind);
        }

        [Fact]
        public void BuildRule_KindSpecifiedUsingIsExplicitlyForbidden_ReturnsRuleWithCorrectKind()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB", version: "12.0.0", withPrivateAssetsAll: true)
                .IsExplicitlyForbidden()
                .Because("A cool rule I made");

            var rule = ruleBuilder.BuildRule();

            Assert.Equal(RuleKind.ExplicitlyForbidden, rule.Kind);
        }

        [Fact]
        public void BuildRule_PrivateAssetsAndVersionNotSpecified_ReturnsRuleWithThoseAsNull()
        {
            var ruleBuilder = ReferenceRule.For("ProjectA")
                .Referencing("ProjectB")
                .IsForbidden()
                .Because("A cool rule I made");

            var rule = ruleBuilder.BuildRule();

            Assert.Null(rule.IsPrivateAssetsAllSet);
            Assert.Null(rule.Version);
        }
    }
}
