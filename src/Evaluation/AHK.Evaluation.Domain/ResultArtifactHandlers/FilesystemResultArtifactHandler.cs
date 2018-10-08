using System;

namespace AHK.Evaluation
{
    /// <summary>
    /// Puts the execution result artifacts into a folder with the student identifier as the subfolder.
    /// The provided path can resolve {date} and {datum}.
    /// </summary>
    public class FilesystemResultArtifactHandler : IResultArtifactHandler
    {
        private readonly string basePath;

        public FilesystemResultArtifactHandler(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
                throw new ArgumentNullException(nameof(basePath));

            this.basePath = resolveBasePath(basePath);

            if (!PathHelper.IsPathValid(basePath))
                throw new ArgumentException("The path is not valid.", nameof(basePath));
        }

        public string GetPathFor(string studentId)
        {
            var path = System.IO.Path.Combine(basePath, studentId);
            if (System.IO.Directory.Exists(path))
                path = System.IO.Path.Combine(basePath, $"{studentId}-{DateTime.Now.ToPathCompatibleString()}");
            System.IO.Directory.CreateDirectory(path);
            return path;
        }

        private static string resolveBasePath(string basePath)
            => basePath
                .Replace("{date}", DateTime.Now.ToPathCompatibleString())
                .Replace("{datum}", DateTime.Now.ToPathCompatibleString());
    }
}
