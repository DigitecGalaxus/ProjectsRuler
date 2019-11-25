using System;
using System.Text;

namespace ProjectReferencesRuler
{
    public class SolutionProjectGuidChecker
    {
        private readonly ISolutionParser solutionParser;

        public SolutionProjectGuidChecker(ISolutionParser solutionParser)
        {
            this.solutionParser = solutionParser;
        }

        public string CheckSolutionProjectGuids(
            string solutionPath,
            string allowedProjectGuid,
            string checkProjectFilesExtensions)
        {
            var checkLog = new StringBuilder();
            foreach (var solutionProject in solutionParser.ExtractSolutionProjects(solutionPath, projectFileExtension: ""))
            {
                if (!solutionProject.IsFolder && solutionProject.ProjectGuid != allowedProjectGuid && solutionProject.ProjectPath.Contains(checkProjectFilesExtensions))
                {
                    checkLog.AppendLine(
                        $"Solution project {solutionProject.ProjectPath} has the project GUID {solutionProject.ProjectGuid}. It should be {allowedProjectGuid}!");
                }
            }

            var complaints = checkLog.ToString();
            if (!string.IsNullOrEmpty(complaints))
            {
                complaints = $"Complaints for the solution: {solutionPath}" + Environment.NewLine + complaints;
            }

            return complaints;
        }
    }
}