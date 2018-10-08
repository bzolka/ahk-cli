using System;
using System.IO;

namespace AHK.TaskRunner
{
    public class TempPathScope : IDisposable
    {
        public readonly string Path;

        public TempPathScope(string path) => this.Path = path;

        public void Dispose()
        {
            if (string.IsNullOrEmpty(Path))
                return;

            if (File.Exists(Path))
                File.Delete(Path);

            if (Directory.Exists(Path))
                Directory.Delete(Path, true);
        }
    }
}
