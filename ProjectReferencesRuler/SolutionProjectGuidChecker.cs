using System;
using System.Text;

namespace ProjectReferencesRuler
{
    [Obsolete(
        "Solution project GUID checks are deprecated because they are specific to classic .sln files. Migrate to .slnx-based checks where possible."
    )]
    public class SolutionProjectGuidChecker
    {
        private readonly ISolutionParser solutionParser;

        public SolutionProjectGuidChecker(ISolutionParser solutionParser)
        {
            this.solutionParser = solutionParser;
        }

        [Obsolete(
            "Classic .sln GUID validation is deprecated. Keep this check only during migration and phase it out after moving to .slnx."
        )]
        public string CheckSolutionProjectGuids(
            string solutionPath,
            string allowedProjectGuid,
            string checkProjectFilesExtensions
        )
        {
            var checkLog = new StringBuilder();
            foreach (
                var solutionProject in solutionParser.ExtractSolutionProjects(
                    solutionPath,
                    projectFileExtension: ""
                )
            )
            {
                if (
                    !solutionProject.IsFolder
                    && solutionProject.ProjectGuid != allowedProjectGuid
                    && solutionProject.ProjectPath.Contains(checkProjectFilesExtensions)
                )
                {
                    checkLog.AppendLine(
                        $"Solution project {solutionProject.ProjectPath} has the project GUID {solutionProject.ProjectGuid}. It should be {allowedProjectGuid}!"
                    );
                }
            }

            var complaints = checkLog.ToString();
            if (!string.IsNullOrEmpty(complaints))
            {
                complaints =
                    $"Complaints for the solution: {solutionPath}"
                    + Environment.NewLine
                    + complaints;
            }

            return complaints;
        }
    }
}
