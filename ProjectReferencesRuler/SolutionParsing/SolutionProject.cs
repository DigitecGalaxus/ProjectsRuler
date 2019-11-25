namespace ProjectReferencesRuler
{
    public class SolutionProject
    {
        public readonly string ProjectGuid;
        public readonly string ProjectPath;
        public readonly bool IsFolder;

        public SolutionProject(
            string projectGuid,
            string projectPath,
            bool isFolder)
        {
            ProjectGuid = projectGuid;
            ProjectPath = projectPath;
            IsFolder = isFolder;
        }
    }
}