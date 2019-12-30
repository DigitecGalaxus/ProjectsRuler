using System.IO;
using System.Linq;
using System.Reflection;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.SolutionParsing;
using Xunit;

namespace ProjectReferencesRuler.Dogfooding
{
    public class ProjectsExistanceCheckTests
    {
        [Fact]
        public void CheckForBrokenReferences()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var solutionDir = Path.Combine(dir, @"../../../../");
            var fullWithoutExternal = Path.Combine(solutionDir, "ProjectsRuler.sln");
            var checker = new ReferencesExistenceChecker(
                new SolutionParser(),
                new CsprojReferencesExtractor());

            var messages = checker.CheckProjectReferencesExistenceInSolution(fullWithoutExternal, "*.csproj").ToList();

            //, $"Check for broken references failed. See messages:\n{string.Join("\n", messages)}"
            Assert.Empty(messages);
        }
    }
}