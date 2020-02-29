using System.Collections.Generic;
using ProjectReferencesRuler.Rules.Project;
using ProjectReferencesRuler.Rules.References;

namespace ProjectReferencesRuler.ProjectParsing
{
    public interface IReferencesExtractor
    {
        /// <summary>
        /// Gets all project references from a csproj file.
        /// </summary>
        /// <param name="csprojPath">Absolute path to the project file.</param>
        /// <returns>Returns all project references file names without path or extension.</returns>
        IEnumerable<Reference> GetProjectReferences(string csprojPath);

        /// <summary>
        /// Gets all project references paths from a csproj file. This is intended for further analysis of the references.
        /// </summary>
        /// <param name="csprojPath">Absolute path to the project file.</param>
        /// <returns>Returns all project references file names with full path and extension.</returns>
        IEnumerable<string> GetProjectReferencePaths(string csprojPath);

        /// <summary>
        /// Gets all (nuget) package references from a csproj file.
        /// </summary>
        /// <param name="csprojPath">Absolute path to the project file.</param>
        /// <returns>Returns all package references without versions or other additional data.</returns>
        IEnumerable<Reference> GetPackageReferences(string csprojPath);

        /// <summary>
        /// Returns project properties needed to apply rules.
        /// </summary>
        /// <param name="csprojPath"></param>
        /// <returns></returns>
        Project GetProjectProperties(string csprojPath);
    }
}
