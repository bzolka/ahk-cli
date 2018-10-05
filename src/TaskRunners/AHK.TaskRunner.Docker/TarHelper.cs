using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace AHK.TaskRunner
{
    internal static class TarHelper
    {
        public static void CreateTarFromDirectory(Stream targetStream, string directory)
        {
            using (var gzipStream = new GZipOutputStream(targetStream))
            {
                gzipStream.IsStreamOwner = false;
                using (var tarArchive = TarArchive.CreateOutputTarArchive(gzipStream))
                {
                    tarArchive.AsciiTranslate = false;
                    tarArchive.IsStreamOwner = false;

                    addDirectoryContentsToTar(directory, directory, tarArchive);
                }
            }
        }

        private static void addDirectoryContentsToTar(string directoryToAdd, string baseDirectory, TarArchive tarArchive)
        {
            foreach (var fileFullPath in Directory.EnumerateFiles(directoryToAdd))
            {
                var e = TarEntry.CreateEntryFromFile(fileFullPath);
                e.Name = getFileNameInTar(baseDirectory, fileFullPath);
                tarArchive.WriteEntry(e, false);
            }

            foreach (var dirFullPath in Directory.EnumerateDirectories(directoryToAdd))
            {
                var e = TarEntry.CreateEntryFromFile(dirFullPath);
                e.Name = getFileNameInTar(baseDirectory, dirFullPath);
                tarArchive.WriteEntry(e, false);

                addDirectoryContentsToTar(dirFullPath, baseDirectory, tarArchive);
            }
        }

        private static string getFileNameInTar(string baseDirectory, string fileFullPath)
            => fileFullPath
                    .Replace(baseDirectory, "")
                    .Replace(Path.DirectorySeparatorChar, '/')
                    .TrimStart('/');

        public static void ExtractTo(Stream tarStream, string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);
            using (var tarArchive = TarArchive.CreateInputTarArchive(tarStream))
            {
                tarArchive.AsciiTranslate = false;
                tarArchive.ExtractContents(destinationDirectory);
            }
        }
    }
}
