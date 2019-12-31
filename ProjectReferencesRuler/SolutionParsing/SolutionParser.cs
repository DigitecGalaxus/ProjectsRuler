using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectReferencesRuler.SolutionParsing
{
    public class SolutionParser : ISolutionParser
    {
        public IEnumerable<SolutionProject> ExtractSolutionProjects(string solutionPath, string projectFileExtension)
        {
            var solutionDir = Path.GetDirectoryName(CleanPath(solutionPath));
            foreach (var line in File.ReadLines(CleanPath(solutionPath)))
            {
                if (line.StartsWith("Project"))
                {
                    var projectPath = ParseProjectPath(line);
                    var projectGuid = CleanPath(ParseProjectGuid(line));
                    if (projectPath != null && projectPath.EndsWith(projectFileExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        yield return new SolutionProject(
                            projectGuid: projectGuid,
                            projectPath: CleanPath(Path.Combine(solutionDir, projectPath)),
                            isFolder: projectGuid == "2150E333-8FDC-42A3-9474-1A3956D46DE8");
                    }
                }
            }
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