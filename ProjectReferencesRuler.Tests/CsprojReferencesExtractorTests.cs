using System.Linq;
using ProjectReferencesRuler.ProjectParsing;
using Xunit;

namespace ProjectReferencesRuler
{
    public class CsprojReferencesExtractorTests
    {
        [Fact]
        public void GetProjectReferences_EmptyProjectFile_NoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor.GetProjectReferences(@"../../../TestProjectFiles/Empty.xml");

            Assert.Empty(references);
        }

        [Fact]
        public void GetProjectReferences_ProjectFileWithoutReferences_NoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor.GetProjectReferences(@"../../../TestProjectFiles/Dg.Returns.Contracts.xml");

            Assert.Empty(references);
        }

        [Fact]
        public void GetProjectReferences_ProjectFileWithReferenceWithoutPath_NoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor
                .GetProjectReferences(@"../../../TestProjectFiles/ReferenceWithoutPath.xml")
                .ToList();

            Assert.Empty(references);
        }

        [Fact]
        public void GetProjectReferences_ProjectFileWithTwoReferences_TwoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor
                .GetProjectReferences(@"../../../TestProjectFiles/Dg.Returns.xml")
                .ToList();

            Assert.NotEmpty(references);
            Assert.Equal(3, references.Count());
        }

        // **************** PackageReferences

        [Fact]
        public void GetPackageReferences_EmptyProjectFile_NoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor.GetPackageReferences(@"../../../TestProjectFiles/Empty.xml");

            Assert.Empty(references);
        }

        [Fact]
        public void GetPackageReferences_ProjectFileWith2Packages_2ReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor.GetPackageReferences(@"../../../TestProjectFiles/Dg.Returns.Contracts.xml").Select(r => r.To).ToList();

            Assert.NotEmpty(references);
            Assert.Equal(2, references.Count);
            Assert.Contains("devinite.Tools.Roslyn.Analyzers.InProcessApiGuideline", references);
            Assert.Contains("Dg", references);
        }

        [Fact]
        public void GetPackageReferences_ProjectFileWithReferenceWithoutPath_NoReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor
                .GetPackageReferences(@"../../../TestProjectFiles/ReferenceWithoutPath.xml")
                .ToList();

            Assert.Empty(references);
        }

        [Fact]
        public void GetPackageReferences_ProjectFileWith4References_4ReferencesReturned()
        {
            var extractor = new CsprojReferencesExtractor();

            var references = extractor
                .GetPackageReferences(@"../../../TestProjectFiles/Dg.Returns.xml")
                .Select(r => r.To)
                .ToList();

            Assert.NotEmpty(references);
            Assert.Equal(4, references.Count);
            Assert.Contains("devinite.Tools.Roslyn.Analyzers.InProcessApiGuideline", references);
            Assert.Contains("Dg", references);
            Assert.Contains("SimpleInjector", references);
            Assert.Contains("Deblazer", references);
        }

        [Theory]
        [InlineData("Dg.Component.Contracts.xml", "netstandard2.0")]
        [InlineData("Dg.Component.xml", "netstandard2.0")]
        [InlineData("Dg.Returns.Contracts.xml", "netstandard2.0")]
        [InlineData("Dg.Returns.xml", "netstandard2.0")]
        [InlineData("Duplicates.xml", "netcoreapp2.1", "netstandard2.0", "net472")]
        [InlineData("Empty.xml")]
        public void GetTargetFrameworks_WithExampleFile_ReturnsTargetFrameworksFromFile(string projectFileName, params string[] expectedTargetFrameworks)
        {
            var extractor = new CsprojReferencesExtractor();

            var projectProperties = extractor.GetProjectProperties(@"../../../TestProjectFiles/" + projectFileName);

            Assert.Equal(expectedTargetFrameworks, projectProperties.TargetFrameworks);
        }

        [Theory]
        [InlineData("Dg.Component.Contracts.xml")]
        [InlineData("Dg.Component.xml", @"$(MonolithRootDir)build/LibraryNoNetStandard.props")]
        [InlineData("Duplicates.xml", @"$(MonolithRootDir)build/LibraryNoNetStandard.props", @"$(MonolithRootDir)build/LibraryNetStandard.props")]
        [InlineData("Empty.xml")]
        public void GetImportedProps_WithExampleFile_ReturnsImportedPropsFromFile(string projectFileName, params string[] expectedImportedProps)
        {
            var extractor = new CsprojReferencesExtractor();

            var projectProperties = extractor.GetProjectProperties(@"../../../TestProjectFiles/" + projectFileName);

            Assert.Equal(expectedImportedProps, projectProperties.ImportedProps.ToList());
        }
    }
}