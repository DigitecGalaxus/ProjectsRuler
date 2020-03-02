using System.IO;
using System.Reflection;
using ProjectReferencesRuler.Rules;
using ProjectReferencesRuler.Rules.References;
using Xunit;

namespace ProjectReferencesRuler.Dogfooding
{
    public class PackageReferenceTests
    {
        [Fact]
        public void AllPackageReferencesHaveToSetPrivateAssetsToAll()
        {
            AssertReferenceRules(
                new ReferenceRule(@"*", @"*", RuleKind.Forbidden, description: "All packages have to have PrivateAssets=\"All\" set.", isPrivateAssetsAllSet: false)
            );
        }

        [Fact]
        public void ProjectsRulerCanOnlyReferenceAllowedPackages()
        {
            AssertReferenceRules(
                new ReferenceRule(@"*", @"Dg*", RuleKind.Forbidden, description: "Please do not couple this project with any company stuff."),
                new ReferenceRule(@"*", @"Chabis*", RuleKind.Forbidden, description: "Please do not couple this project with any company stuff."),
                new ReferenceRule(@"ProjectReferencesRuler", @"*", RuleKind.Forbidden, description: "When possible, please keep the nuget without any other dependencies.")

                // allowed exceptions
            );
        }

        /// <summary>
        /// This whole test doesn't make much sense. The sole purpose is to test the rules. It will make much more sense
        /// when we move from the exact version checking to the minimum version checking.
        /// </summary>
        [Fact]
        public void PackagesShouldNotDowngrade()
        {
            AssertReferenceRules(
                new ReferenceRule(@"*", @"Microsoft.NET.Test.Sdk", RuleKind.Forbidden, description: "Packages should not downgrade.", version: "15.7.0"),
                new ReferenceRule(@"*", @"Moq*", RuleKind.Forbidden, description: "Packages should not downgrade.", version: "4.9.0"),
                new ReferenceRule(@"*", @"xunit", RuleKind.Forbidden, description: "Packages should not downgrade.", version: "2.3.0"),
                new ReferenceRule(@"*", @"xunit.runner.visualstudio", RuleKind.Forbidden, description: "Packages should not downgrade.", version: "2.4.0"),

                // allowed exceptions
                new ReferenceRule(@"ProjectReferencesRuler.Tests", @"xunit.runner.visualstudio", RuleKind.Allowed, description: "The sole purpose of this rule is to test the rule. It doesn't make much sense.", version: "2.4.0")
            );
        }

        [Fact]
        public void ReferenceRulesCanBeCreatedFluentlyToo()
        {
            var rule = ReferenceRule.For("*")
                .Referencing("Chabis.*")
                .IsForbidden()
                .Because("Nothing should reference Chabis")
                .BuildRule();

            AssertReferenceRules(rule);
        }

        private void AssertReferenceRules(params ReferenceRule[] rules)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var solutionDir = Path.Combine(dir, @"../../../");

            var complaints = ProjectsRuler.GetPackageReferencesComplaints(solutionDir, rules);

            Assert.Empty(complaints);
        }
    }
}
