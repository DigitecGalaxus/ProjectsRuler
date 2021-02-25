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
            return GetProjectReferencesComplaints(solutionDir, null, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules).GetComplaintsForProjectReferences();
        }

        public static string GetPackageReferencesComplaints(string solutionDir, params ReferenceRule[] rules)
        {
            return GetPackageReferencesComplaints(solutionDir, null, rules);
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules).GetComplaintsForPackageReferences();
        }

        private static ReferencesRulerRunner GetRunner(
            string solutionDir,
            string excludedProjectsRegex,
            IReadOnlyList<ReferenceRule> rules)
        {
            return new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: solutionDir,
                    filesExtension: "*.csproj",
                    excludedProjectsRegex));
        }
    }
}