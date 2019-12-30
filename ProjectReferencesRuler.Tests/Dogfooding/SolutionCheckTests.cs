using System.IO;
using System.Reflection;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.SolutionParsing;
using Xunit;

namespace ProjectReferencesRuler.Dogfooding
{
    public class SolutionCheckTests
    {
        [Fact]
        public void CheckIfSolutionHasAllValidProjectGuids()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var solutionDir = Path.Combine(dir, @"../../../../");
            var projectGuidChecker = new SolutionProjectGuidChecker(new SolutionParser());
            var complaints = new ProjectFilesRunner(solutionDir, "*.sln").CollectComplaintsForFiles(
                filePath => new[] { projectGuidChecker.CheckSolutionProjectGuids(filePath, allowedProjectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", checkProjectFilesExtensions: ".csproj") });

            Assert.Empty(complaints);
        }
    }
}