using System.Collections.Generic;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler
{
    public static class ProjectsRuler
    {
        public static string GetProjectReferencesComplaints(string solutionDir, params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, rules).GetComplaintsForProjectReferences();
        }

        public static string GetPackageReferencesComplaints(string solutionDir, params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, rules).GetComplaintsForPackageReferences();
        }

        private static ReferencesRulerRunner GetRunner(
            string solutionDir,
            IReadOnlyList<ReferenceRule> rules)
        {
            return new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: solutionDir,
                    filesExtension: "*.csproj"));
        }
    }
}