using System.Collections.Generic;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler
{
    public class ReferenceDuplicatesChecker
    {
        private readonly IReferencesExtractor extractor;
        private readonly IProjectFilesRunner filesRunner;

        public ReferenceDuplicatesChecker(
            IReferencesExtractor extractor,
            IProjectFilesRunner filesRunner)
        {
            this.extractor = extractor;
            this.filesRunner = filesRunner;
        }

        public string CheckProjectReferenceDuplicates()
        {
            return filesRunner.CollectComplaintsForFiles(
                fileName => CheckReferencesDuplicates(
                    extractor.GetProjectReferences(fileName),
                    fileName,
                    "ProjectReference"));
        }

        public string CheckPackageReferenceDuplicates()
        {
            return filesRunner.CollectComplaintsForFiles(
                fileName => CheckReferencesDuplicates(
                    extractor.GetPackageReferences(fileName),
                    fileName,
                    "PackageReference"));
        }

        private IEnumerable<string> CheckReferencesDuplicates(IEnumerable<Reference> references, string fileName, string referenceType)
        {
            var projectReferences = new HashSet<string>();
            foreach (var projectReference in references)
            {
                if (projectReferences.Contains(projectReference.To))
                {
                    yield return $"There is a duplicate {referenceType} {projectReference.To} in {fileName}. Please clean this up!";
                }
                else
                {
                    projectReferences.Add(projectReference.To);
                }
            }
        }
    }
}
