using System;
using System.Collections.Generic;
using Moq;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using Xunit;

namespace ProjectReferencesRuler
{
    public class ReferenceDuplicatesCheckerTests
    {
        [Fact]
        public void CheckProjectReferenceDuplicates_NoProjectDuplicates_ReturnsEmptyString()
        {
            var filesRunnerMock = new Mock<IProjectFilesRunner>();
            filesRunnerMock.Setup(fr => fr.CollectComplaintsForFiles(It.IsAny<Func<string, IEnumerable<string>>>()))
                .Returns((Func<string, IEnumerable<string>> func) => string.Join("\n", func(@"../../../TestProjectFiles/Dg.Component.xml")));
            var checker = new ReferenceDuplicatesChecker(new CsprojReferencesExtractor(), filesRunnerMock.Object);

            var complaints = checker.CheckProjectReferenceDuplicates();

            Assert.Empty(complaints);
        }

        [Fact]
        public void CheckProjectReferenceDuplicates_1ProjectDuplicate_Returns1Complaint()
        {
            var filesRunnerMock = new Mock<IProjectFilesRunner>();
            filesRunnerMock.Setup(fr => fr.CollectComplaintsForFiles(It.IsAny<Func<string, IEnumerable<string>>>()))
                .Returns((Func<string, IEnumerable<string>> func) => string.Join("\n", func(@"../../../TestProjectFiles/Duplicates.xml")));
            var checker = new ReferenceDuplicatesChecker(new CsprojReferencesExtractor(), filesRunnerMock.Object);

            var complaints = checker.CheckProjectReferenceDuplicates();

            Assert.NotEmpty(complaints);
            Assert.Equal(@"There is a duplicate ProjectReference Dg.Component.Contracts in ../../../TestProjectFiles/Duplicates.xml. Please clean this up!", complaints);
        }
        [Fact]
        public void CheckPackageReferenceDuplicates_NoPackageDuplicates_ReturnsEmptyString()
        {
            var filesRunnerMock = new Mock<IProjectFilesRunner>();
            filesRunnerMock.Setup(fr => fr.CollectComplaintsForFiles(It.IsAny<Func<string, IEnumerable<string>>>()))
                .Returns((Func<string, IEnumerable<string>> func) => string.Join("\n", func(@"../../../TestProjectFiles/Empty.xml")));
            var checker = new ReferenceDuplicatesChecker(new CsprojReferencesExtractor(), filesRunnerMock.Object);

            var complaints = checker.CheckPackageReferenceDuplicates();

            Assert.Empty(complaints);
        }

        [Fact]
        public void CheckPackageReferenceDuplicates_1PackageDuplicate_Returns1Complaint()
        {
            var filesRunnerMock = new Mock<IProjectFilesRunner>();
            filesRunnerMock.Setup(fr => fr.CollectComplaintsForFiles(It.IsAny<Func<string, IEnumerable<string>>>()))
                .Returns((Func<string, IEnumerable<string>> func) => string.Join("\n", func(@"../../../TestProjectFiles/Duplicates.xml")));
            var checker = new ReferenceDuplicatesChecker(new CsprojReferencesExtractor(), filesRunnerMock.Object);

            var complaints = checker.CheckPackageReferenceDuplicates();

            Assert.NotEmpty(complaints);
            Assert.Equal(@"There is a duplicate PackageReference Dg in ../../../TestProjectFiles/Duplicates.xml. Please clean this up!", complaints);
        }
    }
}