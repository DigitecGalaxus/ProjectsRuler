using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProjectReferencesRuler.ProjectParsing;

namespace ProjectReferencesRuler
{
    /// <summary>
    /// Checks if the project references fit to references in solutions in the repository.
    /// </summary>
    public class ReferencesExistenceChecker
    {
        private readonly ISolutionParser _solutionParser;
        private readonly IReferencesExtractor _referencesExtractor;

        public ReferencesExistenceChecker(ISolutionParser solutionParser, IReferencesExtractor referencesExtractor)
        {
            _solutionParser = solutionParser;
            _referencesExtractor = referencesExtractor;
        }

        public IEnumerable<string> CheckProjectReferencesExistenceInSolution(string solutionPath, string projectFileExtension)
        {
            if (File.GetAttributes(solutionPath).HasFlag(FileAttributes.Directory))
            {
                throw new ArgumentException($"You passed a folder path {solutionPath} to the {nameof(solutionPath)} parameter. Please pass the solution file path.");
            }

            var messages = new List<string>();
            var absoluteProjectPaths = new HashSet<string>(_solutionParser.ExtractSolutionProjects(solutionPath, projectFileExtension).Select(sp => Path.GetFullPath(sp.ProjectPath)));
            foreach (var projectPath in absoluteProjectPaths)
            {
                var projectDir = Path.GetDirectoryName(projectPath);
                var absoluteReferencePaths = _referencesExtractor.GetProjectReferencePaths(projectPath).Select(p => Path.GetFullPath(Path.Combine(projectDir, p)));
                foreach (var referencePath in absoluteReferencePaths)
                {
                    if (!absoluteProjectPaths.Contains(referencePath))
                    {
                        var shouldBeTowards = absoluteProjectPaths.SingleOrDefault(pp => pp.EndsWith(Path.GetFileName(referencePath)));
                        var message = $"Project {Path.GetFileName(projectPath)} has a broken reference to {referencePath}.";
                        if (string.IsNullOrEmpty(shouldBeTowards))
                        {
                            message += $" This reference is completely missing from the solution {Path.GetFileName(solutionPath)}";
                        }
                        else
                        {
                            message += $" The reference should be towards {shouldBeTowards}";
                        }
                        messages.Add(message);
                    }
                }
            }

            return messages;
        }
    }
}