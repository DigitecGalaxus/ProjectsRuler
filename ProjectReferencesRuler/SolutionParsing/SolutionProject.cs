using System;

namespace ProjectReferencesRuler
{
    public class SolutionProject
    {
        [Obsolete(
            "ProjectGuid is deprecated because it is specific to classic .sln files and will be removed in the next major migration step."
        )]
        public readonly string ProjectGuid;
        public readonly string ProjectPath;
        public readonly bool IsFolder;

        public SolutionProject(string projectGuid, string projectPath, bool isFolder)
        {
#pragma warning disable CS0618 // keep legacy field populated during migration
            ProjectGuid = projectGuid;
#pragma warning restore CS0618
            ProjectPath = projectPath;
            IsFolder = isFolder;
        }
    }
}
