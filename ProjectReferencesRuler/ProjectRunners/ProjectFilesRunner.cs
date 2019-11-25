using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectReferencesRuler.ProjectRunners
{
    public class ProjectFilesRunner : IProjectFilesRunner
    {
        private readonly string _solutionPath;
        private readonly string _filesExtension;

        public ProjectFilesRunner(
            string solutionPath,
            string filesExtension)
        {
            _solutionPath = solutionPath;
            _filesExtension = filesExtension;
        }
        public string CollectComplaintsForFiles(Func<string, IEnumerable<string>> getComplaints)
        {
            var allComplaints = new List<string>();
            foreach (var filePath in Directory.GetFiles(_solutionPath, _filesExtension, SearchOption.AllDirectories))
            {
                allComplaints.AddRange(getComplaints(filePath));
            }

            return string.Join("\n", allComplaints.Where(s => s.Length > 0));
        }
    }
}