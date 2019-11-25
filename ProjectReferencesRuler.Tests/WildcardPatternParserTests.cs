using Xunit;

namespace ProjectReferencesRuler
{
    public class WildcardPatternParserTests
    {
        [Theory]
        [InlineData(@"Dg.*", @"^Dg\..*$")]
        [InlineData(@"devinite.*", @"^devinite\..*$")]
        [InlineData(@"Dg.*.Contracts", @"^Dg\..*\.Contracts$")]
        [InlineData(@"*", @"^.*$")]
        [InlineData(@"Dg.*Test*", @"^Dg\..*Test.*$")]
        [InlineData(@"Dg.*.Erp", @"^Dg\..*\.Erp$")]
        [InlineData(@"Dg.*.Monolith", @"^Dg\..*\.Monolith$")]
        [InlineData(@"Dg.*.Website", @"^Dg\..*\.Website$")]
        [InlineData(@"Dg.Logistics", @"^Dg\.Logistics$")]
        [InlineData(@"Dg.CategoryManagement", @"^Dg\.CategoryManagement$")]
        [InlineData(@"Dg.Finance", @"^Dg\.Finance$")]
        [InlineData(@"Dg.HumanResources", @"^Dg\.HumanResources$")]
        [InlineData(@"Dg.Pdm", @"^Dg\.Pdm$")]
        [InlineData(@"Dg.MessageReceivers", @"^Dg\.MessageReceivers$")]
        [InlineData(@"Dg.Wms", @"^Dg\.Wms$")]
        [InlineData(@"Dg.???", @"^Dg\....$")]
        public void GetRegex_GivenWildcardPattern_ReturnsMatchingRegex(string wildcardPattern, string expectedRegex)
        {
            var parser = new WildcardPatternParser();
            Assert.Equal(expectedRegex, parser.GetRegex(wildcardPattern));
        }
    }
}