using System.IO;
using System.Linq;
using ProjectReferencesRuler.SolutionParsing;
using Xunit;

namespace ProjectReferencesRuler
{
    public class SolutionParserTests
    {
        [Fact]
        public void ExtractProjectPaths_PathInvalid_ThrowsException()
        {
            var path = "this does not work";
            var solutionParser = new SolutionParser();

            var projects = solutionParser.ExtractSolutionProjects(path, ".csproj");

            Assert.Throws<FileNotFoundException>(() => projects.ToList());
        }

        [Fact]
        public void ExtractProjectPaths_NoLineStartsWithProject_ReturnsEmptyEnumerable()
        {
            var path = @"../../../TestSolutionFiles/README.txt";
            var solutionParser = new SolutionParser();

            var projects = solutionParser.ExtractSolutionProjects(path, ".csproj");

            Assert.Empty(projects);
        }

        [Fact]
        public void ExtractProjectPaths_SimpleSln_ReturnsPaths()
        {
            var path = @"../../../TestSolutionFiles/SmallSolution.txt";
            var solutionParser = new SolutionParser();

            var paths = solutionParser.ExtractSolutionProjects(path, ".csproj").Select(sp => sp.ProjectPath).ToList();

            Assert.NotEmpty(paths);
            Assert.Equal(
                new []
                {
                    @"../../../TestSolutionFiles/devinite.PortalSystem/devinite.PortalSystem.csproj",
                    @"../../../TestSolutionFiles/CustomerService/Dg.CustomerService.Contracts/Dg.CustomerService.Contracts.csproj",
                    @"../../../TestSolutionFiles/CustomerService/Dg.CustomerService.Monolith/Dg.CustomerService.Monolith.csproj"
                },
                paths);
        }

        [Fact]
        public void ExtractProjectGuids_SimpleSln_ReturnsGuids()
        {
            var path = @"../../../TestSolutionFiles/SmallSolution.txt";
            var solutionParser = new SolutionParser();

            var paths = solutionParser.ExtractSolutionProjects(path, ".csproj").Select(sp => sp.ProjectGuid).ToList();

            Assert.NotEmpty(paths);
            Assert.Equal(
                new []
                {
                    @"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC",
                    @"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC",
                    @"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
                },
                paths);
        }

        [Fact]
        public void ExtractProjectPaths_SlnWithFolders_ReturnsOnlyProjectPaths()
        {
            var path = @"../../../TestSolutionFiles/SolutionWithFolders.txt";
            var solutionParser = new SolutionParser();

            var paths = solutionParser.ExtractSolutionProjects(path, ".csproj").Select(sp => sp.ProjectPath).ToList();

            Assert.NotEmpty(paths);
            Assert.Equal(
                new []
                {
                    @"../../../TestSolutionFiles/devinite.PortalSystem/devinite.PortalSystem.csproj",
                },
                paths);
        }

        [Fact]
        public void ExtractProjectPaths_LargeSolution_ParsesWithoutErrors()
        {
            var path = @"../../../TestSolutionFiles/LargeSolution.txt";
            var solutionParser = new SolutionParser();

            var projects = solutionParser.ExtractSolutionProjects(path, ".csproj").ToList();

            Assert.NotEmpty(projects);
            Assert.Equal(142, projects.Count);
        }
    }
}