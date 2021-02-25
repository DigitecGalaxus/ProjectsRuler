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
        private readonly string _excludedProjectsRegex; 

        public ProjectFilesRunner(
            string solutionPath,
            string filesExtension,
            string excludedProjectsRegex = null)
        {
            _solutionPath = solutionPath;
            _filesExtension = filesExtension;
            _excludedProjectsRegex = excludedProjectsRegex;
        }
        public string CollectComplaintsForFiles(Func<string, IEnumerable<string>> getComplaints)
        {
            var allComplaints = new List<string>();
            foreach (var filePath in Directory.GetFiles(_solutionPath, _filesExtension, SearchOption.AllDirectories)
                .Where(f => _excludedProjectsRegex is null || !Regex.IsMatch(f, pattern: _excludedProjectsRegex)))
            {
                allComplaints.AddRange(getComplaints(filePath));
            }

            return string.Join("\n", allComplaints.Where(s => s.Length > 0));
        }
    }
}