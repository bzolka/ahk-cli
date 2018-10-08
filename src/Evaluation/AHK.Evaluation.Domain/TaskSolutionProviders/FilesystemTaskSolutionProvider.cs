using System;
using System.Collections.Generic;

namespace AHK.Evaluation
{
    /// <summary>
    /// Provides the paths for the student solutions based on a mapping from StudentId to a path.
    /// </summary>
    public class FilesystemTaskSolutionProvider : ITaskSolutionProvider
    {
        private readonly IDictionary<string, string> paths;

        public FilesystemTaskSolutionProvider(IDictionary<string, string> paths)
        {
            this.paths = paths ?? throw new ArgumentNullException(nameof(paths));

            foreach (var p in paths)
            {
                if (!System.IO.Directory.Exists(p.Value) && !System.IO.File.Exists(p.Value))
                    throw new ArgumentException($"Solution path '{p}' (for student {p.Key}) does not exist.");
            }
        }

        public string GetSolutionPath(string studentId) => paths[studentId];
    }
}
