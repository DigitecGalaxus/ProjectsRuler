using System.Collections.Generic;

namespace ProjectReferencesRuler
{
    public interface ISolutionParser
    {
        /// <summary>
        /// Extracts solution projects with relative paths from the solution file.
        /// </summary>
        IEnumerable<SolutionProject> ExtractSolutionProjects(string solutionPath, string projectFileExtension);
    }
}