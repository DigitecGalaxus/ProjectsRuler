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
            return GetProjectReferencesComplaints(solutionDir, null, shouldComplainAboutUnusedRules: false, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            bool shouldComplainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetProjectReferencesComplaints(solutionDir, null, shouldComplainAboutUnusedRules, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules)
        {
            return GetProjectReferencesComplaints(solutionDir, excludedProjects, shouldComplainAboutUnusedRules: false, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool shouldComplainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules, shouldComplainAboutUnusedRules).GetComplaintsForProjectReferences();
        }

        public static string GetPackageReferencesComplaints(string solutionDir, params ReferenceRule[] rules)
        {
            return GetPackageReferencesComplaints(solutionDir, null, shouldComplainAboutUnusedRules: false, rules);
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            bool shouldComplainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetPackageReferencesComplaints(solutionDir, null, shouldComplainAboutUnusedRules, rules);
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules)
        {
            return GetPackageReferencesComplaints(solutionDir, excludedProjects, shouldComplainAboutUnusedRules: false, rules);
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool shouldComplainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules, shouldComplainAboutUnusedRules).GetComplaintsForPackageReferences();
        }

        private static ReferencesRulerRunner GetRunner(
            string solutionDir,
            string excludedProjectsRegex,
            IReadOnlyList<ReferenceRule> rules,
            bool shouldComplainAboutUnusedRules)
        {
            return new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules,
                    shouldComplainAboutUnusedRules: shouldComplainAboutUnusedRules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: solutionDir,
                    filesExtension: "*.csproj",
                    excludedProjectsRegex));
        }
    }
}