using System.Collections.Generic;

namespace ProjectReferencesRuler.Rules.Project
{
    public class Project
    {
        public readonly string Name;
        public readonly IReadOnlyList<string> ImportedProps;
        public readonly IReadOnlyList<string> TargetFrameworks;

        // This might become a thing, but for now, let's leave that away.
//        private readonly IReadOnlyList<Reference> projectReferences;
//        private readonly IReadOnlyList<Reference> packageReferences;

        public Project(string name, IReadOnlyList<string> importedProps, IReadOnlyList<string> targetFrameworks)
        {
            Name = name;
            ImportedProps = importedProps;
            TargetFrameworks = targetFrameworks;
        }
    }
}