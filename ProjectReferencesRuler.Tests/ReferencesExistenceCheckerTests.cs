using System;
using System.Linq;
using Moq;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.SolutionParsing;
using Xunit;

namespace ProjectReferencesRuler
{
    public class ReferencesExistenceCheckerTests
    {
        [Fact]
        public void CheckProjectReferencesInSolution_OneProjectInSolutionWithoutReferences_AllGood()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[] { new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "SomeProjectFilePath", isFolder: false) });
            var referencesExtractor = new Mock<IReferencesExtractor>();
            var checker = new ReferencesExistenceChecker(solutionParserMock.Object, referencesExtractor.Object);

            var messages = checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/README.txt", ".xml");

            Assert.Empty(messages);
        }
        [Fact]
        public void CheckProjectReferencesInSolution_MultipleProjectAllowedProjectsInSolution_AllGood()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            solutionParserMock.Setup(sp => sp.ExtractSolutionProjects(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[]
                {
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "SomeProjectFilePath1.csproj", isFolder: false),
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "SomeProjectFilePath2.csproj", isFolder: false),
                    new SolutionProject(projectGuid: "9A19103F-16F7-4668-BE54-9A1E7A4F7556", projectPath: "SomeProjectFilePath3.csproj", isFolder: false)
                });
            var referencesExtractor = new Mock<IReferencesExtractor>();
            var checker = new ReferencesExistenceChecker(solutionParserMock.Object, referencesExtractor.Object);

            var messages = checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/README.txt", ".xml");

            Assert.Empty(messages);
        }

        [Fact]
        public void CheckProjectReferencesInSolution_TwoProjectsInMockedSolutionOneReferencesOther_AllGood()
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
            var checker = new ReferencesExistenceChecker(solutionParserMock.Object, referencesExtractorMock.Object);

            var messages = checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/README.txt", ".xml");

            Assert.Empty(messages);
        }

        [Fact]
        public void CheckProjectReferencesInSolution_TwoProjectsInSolutionOneReferencesOther_AllGood()
        {
            var solutionParser = new SolutionParser();
            var referencesExtractorMock = new CsprojReferencesExtractor();
            var checker = new ReferencesExistenceChecker(solutionParser, referencesExtractorMock);

            var messages =
                checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/SmallValidTestSolution.txt",
                    ".xml");

            Assert.Empty(messages);
        }

        [Fact]
        public void CheckProjectReferencesInSolution_TwoProjectsInSolutionOneReferenceWrong_OneMessageReturned()
        {
            var solutionParser = new SolutionParser();
            var referencesExtractorMock = new CsprojReferencesExtractor();
            var checker = new ReferencesExistenceChecker(solutionParser, referencesExtractorMock);

            var messages =
                checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/SmallInvalidTestSolution.txt",
                    ".xml");

            // this will not work on all machines.
//            Assert.Equal(new[]
//            {
//                @"Project Dg.Returns.xml has a broken reference to C:/Development/MonolithRuler/Dg.DgConsolidate/Dg.DgConsolidate.csproj. This reference is completely missing from the solution SmallInvalidTestSolution.txt",
//                @"Project Dg.Returns.xml has a broken reference to C:/Development/MonolithRuler/Dg.InversionOfControl/Dg.InversionOfControl.csproj. This reference is completely missing from the solution SmallInvalidTestSolution.txt",
//                @"Project Dg.Returns.xml has a broken reference to C:/Development/MonolithRuler/ProjectReferencesRuler.Tests/Dg.Returns.Contracts/Dg.Returns.Contracts.csproj. This reference is completely missing from the solution SmallInvalidTestSolution.txt"
//            }, messages);
            Assert.Equal(3, messages.Count());
        }

        [Fact]
        public void CheckProjectReferencesInSolution_FolderPathPassedForSolution_ThrowsException()
        {
            var solutionParserMock = new Mock<ISolutionParser>();
            var referencesExtractor = new Mock<IReferencesExtractor>();
            var checker = new ReferencesExistenceChecker(solutionParserMock.Object, referencesExtractor.Object);

            Assert.Throws<ArgumentException>(() => checker.CheckProjectReferencesExistenceInSolution(@"../../../TestSolutionFiles/", ".xml"));
        }
    }
}