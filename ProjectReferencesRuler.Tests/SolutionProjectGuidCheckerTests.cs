using Moq;
using ProjectReferencesRuler.ProjectParsing;
using Xunit;

namespace ProjectReferencesRuler
{
    public class SolutionProjectGuidCheckerTests
    {
        [Fact]
        public void CheckSolutionProjectGuids_AllGuidsAllowed_AllGood()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "project1", isFolder: false),
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "project2", isFolder: false)
                });
            var referencesExtractorMock = new Mock<IReferencesExtractor>();
            referencesExtractorMock.Setup(re => re.GetProjectReferencePaths("project1"))
                .Returns(new[] {"project2"});
            var checker = new SolutionProjectGuidChecker(solutionParserMock.Object);

            var messages = checker.CheckSolutionProjectGuids(@"../../../TestSolutionFiles\README.txt", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", ".csproj");

            Assert.Empty(messages);
        }

        [Fact]
        public void CheckSolutionProjectGuids_OneGuidNotAllowed_OneComplaint()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "project1.csproj", isFolder: false),
                    new SolutionProject(projectGuid: "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", projectPath: "project2.csproj", isFolder: false)
                });
            var referencesExtractorMock = new Mock<IReferencesExtractor>();
            referencesExtractorMock.Setup(re => re.GetProjectReferencePaths("project1"))
                .Returns(new[] {"project2"});
            var checker = new SolutionProjectGuidChecker(solutionParserMock.Object);

            var messages = checker.CheckSolutionProjectGuids(@"../../../TestSolutionFiles\README.txt", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", ".csproj");

            Assert.NotEmpty(messages);
        }

        [Fact]
        public void CheckSolutionProjectGuids_OneProjectIsFolder_AllGood()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "project1", isFolder: false),
                    new SolutionProject(projectGuid: "2150E333-8FDC-42A3-9474-1A3956D46DE8", projectPath: "project2", isFolder: true)
                });
            var referencesExtractorMock = new Mock<IReferencesExtractor>();
            referencesExtractorMock.Setup(re => re.GetProjectReferencePaths("project1"))
                .Returns(new[] {"project2"});
            var checker = new SolutionProjectGuidChecker(solutionParserMock.Object);

            var messages = checker.CheckSolutionProjectGuids(@"../../../TestSolutionFiles\README.txt", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", ".csproj");

            Assert.Empty(messages);
        }

        [Fact]
        public void CheckSolutionProjectGuids_TwoGuidsNotAllowedOnlyOneCsproj_OneComplaint()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    new SolutionProject(projectGuid: "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", projectPath: "project1.vcxproj", isFolder: false),
                    new SolutionProject(projectGuid: "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC", projectPath: "project2.csproj", isFolder: false)
                });
            var referencesExtractorMock = new Mock<IReferencesExtractor>();
            referencesExtractorMock.Setup(re => re.GetProjectReferencePaths("project1"))
                .Returns(new[] {"project2"});
            var checker = new SolutionProjectGuidChecker(solutionParserMock.Object);

            var messages = checker.CheckSolutionProjectGuids(@"../../../TestSolutionFiles\README.txt", "9A19103F-16F7-4668-BE54-9A1E7A4F7556", ".csproj");

            Assert.NotEmpty(messages);
            Assert.Contains("project2.csproj", messages);
            Assert.DoesNotContain("project1.vcxproj", messages);
        }
    }
}