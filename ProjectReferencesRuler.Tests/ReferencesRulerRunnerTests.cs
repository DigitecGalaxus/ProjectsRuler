using Moq;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules.References;
using Xunit;

namespace ProjectReferencesRuler
{
    public class ReferencesRulerRunnerTests
    {
        [Fact]
        public void GetComplaintsForProjectReferences_GivenTestProjectFilesDirectory_ExtractsReferencesFromAllGivenFiles()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            var rulerMock = new Mock<IReferencesRuler>();
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForProjectReferences();

            Assert.True(string.IsNullOrEmpty(complaints));
            extractorMock.Verify(e => e.GetProjectReferences(It.IsAny<string>()), Times.Exactly(7));
        }

        [Fact]
        public void GetComplaintsForProjectReferences_RulerHasNoComplaints_ReturnsNoComplaints()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            extractorMock
                .Setup(e => e.GetProjectReferences(It.IsAny<string>()))
                .Returns(new[] { new Reference(from: "source", to: "someReference", isPrivateAssetsAllSet: true, versionOrNull: null), });
            var rulerMock = new Mock<IReferencesRuler>();
            rulerMock
                .Setup(r => r.GiveMeComplaints(It.IsAny<Reference>()))
                .Returns(string.Empty);
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForProjectReferences();

            Assert.True(string.IsNullOrEmpty(complaints));
            rulerMock.Verify(e => e.GiveMeComplaints(It.IsAny<Reference>()), Times.Exactly(7));
        }

        [Fact]
        public void GetComplaintsForProjectReferences_RulerHasComplaints_RunnerCollectsThemAll()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            extractorMock
                .Setup(e => e.GetProjectReferences(It.IsAny<string>()))
                .Returns(new[] { new Reference(from: "source", to: "someReference", isPrivateAssetsAllSet: true, versionOrNull: null), });
            var rulerMock = new Mock<IReferencesRuler>();
            rulerMock
                .Setup(r => r.GiveMeComplaints(It.IsAny<Reference>()))
                .Returns("Aarrr!");
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForProjectReferences();

            Assert.Equal("Aarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!", complaints);
        }
        [Fact]
        public void GetComplaintsForPackageReferences_GivenTestProjectFilesDirectory_ExtractsReferencesFromAllGivenFiles()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            var rulerMock = new Mock<IReferencesRuler>();
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForPackageReferences();

            Assert.True(string.IsNullOrEmpty(complaints));
            extractorMock.Verify(e => e.GetPackageReferences(It.IsAny<string>()), Times.Exactly(7));
        }

        [Fact]
        public void GetComplaintsForPackageReferences_RulerHasNoComplaints_ReturnsNoComplaints()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            extractorMock
                .Setup(e => e.GetPackageReferences(It.IsAny<string>()))
                .Returns(new[] { new Reference(from: "source", to: "someReference", isPrivateAssetsAllSet: true, versionOrNull: null), });
            var rulerMock = new Mock<IReferencesRuler>();
            rulerMock
                .Setup(r => r.GiveMeComplaints(It.IsAny<Reference>()))
                .Returns(string.Empty);
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForPackageReferences();

            Assert.True(string.IsNullOrEmpty(complaints));
            rulerMock.Verify(e => e.GiveMeComplaints(It.IsAny<Reference>()), Times.Exactly(7));
        }

        [Fact]
        public void GetComplaintsForPackageReferences_RulerHasComplaints_RunnerCollectsThemAll()
        {
            var extractorMock = new Mock<IReferencesExtractor>();
            extractorMock
                .Setup(e => e.GetPackageReferences(It.IsAny<string>()))
                .Returns(new[] { new Reference(from: "source", to: "someReference", isPrivateAssetsAllSet: true, versionOrNull: null), });
            var rulerMock = new Mock<IReferencesRuler>();
            rulerMock
                .Setup(r => r.GiveMeComplaints(It.IsAny<Reference>()))
                .Returns("Aarrr!");
            var runner = new ReferencesRulerRunner(
                extractor: extractorMock.Object,
                referencesRuler: rulerMock.Object,
                filesRunner: new ProjectFilesRunner(
                    solutionPath: @"../../../TestProjectFiles/",
                    filesExtension: "*.xml"));

            var complaints = runner.GetComplaintsForPackageReferences();

            Assert.Equal("Aarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!\nAarrr!", complaints);
        }
    }
}
