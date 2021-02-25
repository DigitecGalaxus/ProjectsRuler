using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProjectReferencesRuler.ProjectRunners
{
    public class ProjectFilesRunner : IProjectFilesRunner
    {
        private readonly string _solutionPath;
        private readonly string _filesExtension;
        private readonly Regex _excludedProjects; 

        public ProjectFilesRunner(
            string solutionPath,
            string filesExtension,
            Regex excludedProjects = null)
        {
            _solutionPath = solutionPath;
            _filesExtension = filesExtension;
            _excludedProjects = excludedProjects;
        }
        public string CollectComplaintsForFiles(Func<string, IEnumerable<string>> getComplaints)
        {
            var allComplaints = new List<string>();
            foreach (var filePath in Directory.GetFiles(_solutionPath, _filesExtension, SearchOption.AllDirectories)
                .Where(f => _excludedProjects is null || !_excludedProjects.IsMatch(f)))
            {
                allComplaints.AddRange(getComplaints(filePath));
            }

            return string.Join("\n", allComplaints.Where(s => s.Length > 0));
        }
    }
}