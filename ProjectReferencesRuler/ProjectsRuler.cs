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
            bool complainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetProjectReferencesComplaints(solutionDir, null, complainAboutUnusedRules, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules)
        {
            return GetProjectReferencesComplaints(solutionDir, excludedProjects, complainAboutUnusedRules: false, rules);
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool complainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules, complainAboutUnusedRules).GetComplaintsForProjectReferences();
        }

        public static string GetPackageReferencesComplaints(string solutionDir, params ReferenceRule[] rules)
        {
            return GetPackageReferencesComplaints(solutionDir, null, complainAboutUnusedRules: false, rules);
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
            return GetPackageReferencesComplaints(solutionDir, excludedProjects, complainAboutUnusedRules: false, rules);
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool complainAboutUnusedRules,
            params ReferenceRule[] rules)
        {
            return GetRunner(solutionDir, excludedProjects, rules, complainAboutUnusedRules).GetComplaintsForPackageReferences();
        }

        private static ReferencesRulerRunner GetRunner(
            string solutionDir,
            string excludedProjectsRegex,
            IReadOnlyList<ReferenceRule> rules,
            bool complainAboutUnusedRules)
        {
            return new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules,
                    complainAboutUnusedRules: complainAboutUnusedRules),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: solutionDir,
                    filesExtension: "*.csproj",
                    excludedProjectsRegex));
        }
    }
}