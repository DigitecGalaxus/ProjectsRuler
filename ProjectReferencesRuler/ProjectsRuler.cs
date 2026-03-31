using System.Collections.Generic;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.ProjectRunners;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler
{
    public static class ProjectsRuler
    {
        public static string GetProjectReferencesComplaints(
            string solutionDir,
            params ReferenceRule[] rules
        )
        {
            return GetProjectReferencesComplaints(
                solutionDir,
                null,
                reportUnusedExceptionRules: false,
                rules
            );
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            bool reportUnusedExceptionRules,
            params ReferenceRule[] rules
        )
        {
            return GetProjectReferencesComplaints(
                solutionDir,
                null,
                reportUnusedExceptionRules,
                rules
            );
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules
        )
        {
            return GetProjectReferencesComplaints(
                solutionDir,
                excludedProjects,
                reportUnusedExceptionRules: false,
                rules
            );
        }

        public static string GetProjectReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool reportUnusedExceptionRules,
            params ReferenceRule[] rules
        )
        {
            return GetRunner(solutionDir, excludedProjects, rules, reportUnusedExceptionRules)
                .GetComplaintsForProjectReferences();
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            params ReferenceRule[] rules
        )
        {
            return GetPackageReferencesComplaints(
                solutionDir,
                null,
                reportUnusedExceptionRules: false,
                rules
            );
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            bool shouldReportUnusedExceptionRules,
            params ReferenceRule[] rules
        )
        {
            return GetPackageReferencesComplaints(
                solutionDir,
                null,
                shouldReportUnusedExceptionRules,
                rules
            );
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects = null,
            params ReferenceRule[] rules
        )
        {
            return GetPackageReferencesComplaints(
                solutionDir,
                excludedProjects,
                reportUnusedExceptionRules: false,
                rules
            );
        }

        public static string GetPackageReferencesComplaints(
            string solutionDir,
            string excludedProjects,
            bool reportUnusedExceptionRules,
            params ReferenceRule[] rules
        )
        {
            return GetRunner(solutionDir, excludedProjects, rules, reportUnusedExceptionRules)
                .GetComplaintsForPackageReferences();
        }

        private static ReferencesRulerRunner GetRunner(
            string solutionDir,
            string excludedProjectsRegex,
            IReadOnlyList<ReferenceRule> rules,
            bool reportUnusedExceptionRules
        )
        {
            return new ReferencesRulerRunner(
                extractor: new CsprojReferencesExtractor(),
                referencesRuler: new ReferencesRuler(
                    patternParser: new WildcardPatternParser(),
                    rules: rules,
                    reportUnusedExceptionRules: reportUnusedExceptionRules
                ),
                filesRunner: new ProjectFilesRunner(
                    solutionPath: solutionDir,
                    filesExtension: "*.csproj",
                    excludedProjectsRegex
                )
            );
        }
    }
}
