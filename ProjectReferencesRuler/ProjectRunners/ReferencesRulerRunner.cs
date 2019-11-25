using System;
using System.Collections.Generic;
using System.Linq;
using ProjectReferencesRuler.ProjectParsing;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler.ProjectRunners
{
    public class ReferencesRulerRunner
    {
        private readonly IReferencesExtractor _extractor;
        private readonly IReferencesRuler referencesRuler;
        private readonly IProjectFilesRunner _filesRunner;

        public ReferencesRulerRunner(
            IReferencesExtractor extractor,
            IReferencesRuler referencesRuler,
            IProjectFilesRunner filesRunner)
        {
            _extractor = extractor;
            this.referencesRuler = referencesRuler;
            _filesRunner = filesRunner;
        }

        public string GetComplaintsForProjectReferences()
        {
            return GetComplaintsForReferences(filePath => _extractor.GetProjectReferences(filePath));
        }

        public string GetComplaintsForPackageReferences()
        {
            return GetComplaintsForReferences(filePath => _extractor.GetPackageReferences(filePath));
        }

        private string GetComplaintsForReferences(Func<string, IEnumerable<Reference>> getReferences)
        {
            return _filesRunner.CollectComplaintsForFiles(fileName => GetReferencesComplaints(getReferences, fileName));
        }

        private IEnumerable<string> GetReferencesComplaints(
            Func<string, IEnumerable<Reference>> getReferences,
            string filePath)
        {
            return getReferences(filePath)
                .Select(reference => referencesRuler.GiveMeComplaints(reference))
                .Where(complaints => !string.IsNullOrEmpty(complaints));
        }
    }
}