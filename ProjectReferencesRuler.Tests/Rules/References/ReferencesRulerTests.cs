using System.Collections.Generic;
using Xunit;

namespace ProjectReferencesRuler.Rules.References
{
    public class ReferencesRulerTests
    {
        [Fact]
        public void GiveMeComplaints_NoRules_NoComplaints()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new List<ReferenceRule>());

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.True(string.IsNullOrEmpty(complaints));
        }

        [Fact]
        public void GiveMeComplaints_ForbiddenExplicitRule_ReturnsComplaints()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []{ new ReferenceRule("a", "b", RuleKind.Forbidden, description: "no no") });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.False(string.IsNullOrEmpty(complaints));
            Assert.Equal("Reference from a to b broke:\nno no", complaints);
        }

        [Fact]
        public void GiveMeComplaints_ForbiddenGeneralRule_ReturnsComplaints()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []{ new ReferenceRule(".*", "b", RuleKind.Forbidden, description: "no no") });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.False(string.IsNullOrEmpty(complaints));
            Assert.Equal("Reference from a to b broke:\nno no", complaints);
        }

        [Fact]
        public void GiveMeComplaints_ForbiddenExplicitRuleWithExplicitOverridingRule_NoComplaints()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []
                {
                    new ReferenceRule("a", "b", RuleKind.Forbidden, description: "no no"),
                    new ReferenceRule("a", "b", RuleKind.Allowed, description: "yes yes")
                });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.True(string.IsNullOrEmpty(complaints));
        }

        [Fact]
        public void GiveMeComplaints_ForbiddenGeneralRuleWithExplicitOverridingRule_NoComplaints()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []
                {
                    new ReferenceRule("*", "b", RuleKind.Forbidden, description: "no no"),
                    new ReferenceRule("a", "b", RuleKind.Allowed, description: "yes yes")
                });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.True(string.IsNullOrEmpty(complaints));
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(null, true)]
        public void GiveMeComplaints_ForbiddenExplicitRuleWithPrivateAssets_MatchesPrivateAssetsToo(bool? isPrivateAssetsAllSet, bool hasComplaints)
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []{ new ReferenceRule("a", "b", RuleKind.Forbidden, description: "no no", isPrivateAssetsAllSet: isPrivateAssetsAllSet) });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.Equal(hasComplaints, !string.IsNullOrEmpty(complaints));
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(null, true)]
        public void GiveMeComplaints_ForbiddenExplicitRuleWithPrivateAssetsAndGenericRule_MatchesPrivateAssetsToo(bool? isPrivateAssetsAllSet, bool hasComplaints)
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []{ new ReferenceRule(".*", ".*", RuleKind.Forbidden, description: "no no", isPrivateAssetsAllSet: isPrivateAssetsAllSet) });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: null));

            Assert.Equal(hasComplaints, !string.IsNullOrEmpty(complaints));
        }

        [Theory]
        [InlineData("1.2.3", true)]
        [InlineData("3.2.1", false)]
        [InlineData(null, true)]
        public void GiveMeComplaints_ForbiddenExplicitRuleWithVersion_MatchesVersion(string version, bool hasComplaints)
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []{ new ReferenceRule(".*", ".*", RuleKind.Forbidden, description: "no no", version: version) });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: "1.2.3"));

            Assert.Equal(hasComplaints, !string.IsNullOrEmpty(complaints));
        }

        [Fact]
        public void GiveMeComplaints_ForbiddenExplicitRuleWithVersionAndException_MatchesVersionAndAllowsForTheException()
        {
            var ruler = new ReferencesRuler(
                patternParser: new RegexPatternParser(),
                rules: new []
                {
                    new ReferenceRule(".*", ".*", RuleKind.Forbidden, description: "no no", version: "3.2.1"),
                    new ReferenceRule(".*", ".*", RuleKind.Allowed, description: "no no", version: "1.2.3")
                });

            var complaints = ruler.GiveMeComplaints(new Reference(from: "a", to: "b", isPrivateAssetsAllSet: true, versionOrNull: "1.2.3"));

            Assert.True(string.IsNullOrEmpty(complaints));
        }
    }
}
