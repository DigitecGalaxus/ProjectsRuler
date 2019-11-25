using System;
using System.Collections.Generic;

namespace ProjectReferencesRuler.ProjectRunners
{
    public interface IProjectFilesRunner
    {
        string CollectComplaintsForFiles(Func<string, IEnumerable<string>> getComplaints);
    }
}