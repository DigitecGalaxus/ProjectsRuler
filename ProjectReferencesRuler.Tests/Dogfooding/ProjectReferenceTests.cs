using System.IO;
using System.Reflection;
using ProjectReferencesRuler.Rules;
using ProjectReferencesRuler.Rules.References;
using Xunit;

namespace ProjectReferencesRuler.Dogfooding
{
    public class ProjectReferenceTests
    {
        [Fact]
        public void RulerProjectsShouldNotReferenceOtherProjects()
        {
            AssertReferenceRules(
                new ReferenceRule(@"*", @"*", RuleKind.Forbidden, description: "Projects shouldn't reference other projects."),

                // exceptions
                new ReferenceRule(@"*.Tests", @"*", RuleKind.Allowed, description: "Test projects must reference other projects in order to test them."),
                new ReferenceRule(@"*.Console", @"*", RuleKind.Allowed, description: "Console projects must reference other projects in order to test them."),

                // explicit override
                new ReferenceRule(@"*.Console", @"*.Tests", RuleKind.ExplicitlyForbidden, description: "Console projects must not reference test projects.")
            );
        }
        [Fact]
        public void AllProjectsShouldHavePrivateAssetsSetOnProjectReferences()
        {
            AssertReferenceRules(
                new ReferenceRule(@"*", @"*", RuleKind.Forbidden, description: "All references have to have PrivateAssets=\"All\" set.", isPrivateAssetsAllSet: false)
            );
        }

        private void AssertReferenceRules(params ReferenceRule[] rules)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var solutionDir = Path.Combine(dir, @"../../../../");

            var complaints = ProjectsRuler.GetProjectReferencesComplaints(solutionDir, rules);

            Assert.Empty(complaints);
        }
    }
}