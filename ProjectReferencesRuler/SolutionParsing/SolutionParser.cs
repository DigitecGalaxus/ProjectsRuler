using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProjectReferencesRuler.SolutionParsing
{
    public class SolutionParser : ISolutionParser
    {
        public IEnumerable<SolutionProject> ExtractSolutionProjects(
            string solutionPath,
            string projectFileExtension
        )
        {
            return Path.GetExtension(solutionPath).ToLowerInvariant() switch
            {
                // .xml and .txt are used for tests
                ".slnx" or ".xml" => ExtractSlnxSolutionProjects(
                    solutionPath,
                    projectFileExtension
                ),
                // Legacy support for classic .sln format during migration to .slnx.
                ".sln" or ".txt" => ExtractSlnSolutionProjects(solutionPath, projectFileExtension),
                _ => throw new NotSupportedException(
                    $"Solution type not supported: {solutionPath}"
                ),
            };
        }

        private IEnumerable<SolutionProject> ExtractSlnSolutionProjects(
            string solutionPath,
            string projectFileExtension
        )
        {
            bool atLeastOneReferenceFound = false;
            var solutionDir = Path.GetDirectoryName(CleanPath(solutionPath))!;
            foreach (var line in File.ReadLines(CleanPath(solutionPath)))
            {
                if (line.StartsWith("Project"))
                {
                    var projectPath = ParseProjectPath(line);
                    var projectGuid = CleanPath(ParseProjectGuid(line));
                    if (
                        projectPath != null
                        && projectPath.EndsWith(
                            projectFileExtension,
                            StringComparison.InvariantCultureIgnoreCase
                        )
                    )
                    {
                        atLeastOneReferenceFound = true;
                        yield return new SolutionProject(
                            projectGuid: projectGuid,
                            projectPath: CleanPath(Path.Combine(solutionDir, projectPath)),
                            isFolder: projectGuid == "2150E333-8FDC-42A3-9474-1A3956D46DE8"
                        );
                    }
                }
            }

            if (!atLeastOneReferenceFound)
            {
                throw new InvalidOperationException(
                    $"No project references were found in the solution {solutionPath}."
                );
            }
        }

        private IEnumerable<SolutionProject> ExtractSlnxSolutionProjects(
            string solutionPath,
            string projectFileExtension
        )
        {
            var solutionDir = Path.GetDirectoryName(CleanPath(solutionPath))!;
            var doc = new XmlDocument();
            doc.Load(solutionPath);

            var projects = doc.GetElementsByTagName("Project")
                .OfType<XmlNode>()
                .Select(e => e.Attributes?["Path"].Value)
                .Where(p =>
                    !string.IsNullOrEmpty(p)
                    && p.EndsWith(projectFileExtension, StringComparison.InvariantCultureIgnoreCase)
                )
                .Select(p => new SolutionProject(
                    "",
                    CleanPath(Path.Combine(solutionDir, p)),
                    false
                ))
                .ToList();

            if (!projects.Any())
            {
                throw new InvalidOperationException(
                    $"No project references were found in the solution {solutionPath}."
                );
            }

            return projects;
        }

        private string ParseProjectGuid(string line)
        {
            return line.Substring(10, 36);
        }

        /// <summary>
        /// Parses the solution file line with the project.
        /// </summary>
        /// <param name="line">Line in format Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Dg.TestsWithDb", "Dg.TestsWithDb\Dg.TestsWithDb.csproj", "{AB999FEB-AB60-4857-840A-4E0FEB9F145D}"</param>
        /// <returns></returns>
        private string ParseProjectPath(string line)
        {
            // the first 53 characters are always fix in the format Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") =
            var projectContent = line.Substring(53);

            // takes the middle part of "Dg.TestsWithDb", "Dg.TestsWithDb\Dg.TestsWithDb.csproj", "{AB999FEB-AB60-4857-840A-4E0FEB9F145D}"
            var pathInParenthesis = projectContent.Split(',')[1];

            // removes the parenthesis
            return pathInParenthesis.Trim().Substring(1, pathInParenthesis.Length - 3);
        }

        /// <summary>
        /// Replaces \ with / in order for this same code to work on both Windows and Linux.
        /// </summary>
        private static string CleanPath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
