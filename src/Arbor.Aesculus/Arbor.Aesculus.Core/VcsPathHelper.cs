using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arbor.Aesculus.Core
{
    public static class VcsPathHelper
    {
        public static string TryFindVcsRootPath(string startDirectory = null)
        {
            var directoryPath = !string.IsNullOrWhiteSpace(startDirectory)
                                    ? startDirectory
                                    : AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException(string.Format("The start directory '{0}' does not exist",
                                                                   directoryPath));
            }

            var startDir = new DirectoryInfo(directoryPath);

            DirectoryInfo rootDirectory = NavigateToSourceRoot(startDir);


            return rootDirectory == null ? null : rootDirectory.FullName;
        }

        public static string FindVcsRootPath(string startDirectory = null)
        {
            var rootDirectory = TryFindVcsRootPath(startDirectory);

            if (!string.IsNullOrWhiteSpace(rootDirectory))
            {
                return rootDirectory;
            }

            throw new DirectoryNotFoundException("Could not find the source root");
        }

        static DirectoryInfo NavigateToSourceRoot(DirectoryInfo currentDirectory)
        {
            if (currentDirectory.Parent == null)
            {
                return null;
            }

            var sourceRootDirectoryNames = new List<string> {".git", ".hg"};
            var sourceRootFileNames = new List<string> {".deployment", ".gitattributes"};

            Func<DirectoryInfo, bool> isNamedRoot =
                dir =>
                sourceRootDirectoryNames.Any(rootName =>
                                             dir.Name.Equals(rootName, StringComparison.InvariantCultureIgnoreCase));

            var directoryCriterias = new List<Func<DirectoryInfo, bool>>
                                         {
                                             isNamedRoot
                                         };

            Func<FileInfo, bool> isRootFile =
                file =>
                sourceRootFileNames.Any(rootFile =>
                                        file.Name.Equals(rootFile, StringComparison.InvariantCultureIgnoreCase));

            var fileCriterias = new List<Func<FileInfo, bool>>
                                    {
                                        isRootFile
                                    };


            bool directoryHasSourceRoot = currentDirectory.EnumerateDirectories()
                                                          .Any(dir => sourceRootDirectoryNames.Any(pattern =>
                                                                                                   dir.Name.Equals(
                                                                                                       pattern,
                                                                                                       StringComparison
                                                                                                           .InvariantCultureIgnoreCase)));
            if (directoryHasSourceRoot)
            {
                return currentDirectory;
            }


            bool isRootDir = directoryCriterias.Any(filter => filter(currentDirectory));

            if (isRootDir)
            {
                return currentDirectory;
            }

            bool hasRootFile = currentDirectory.EnumerateFiles().Any(file => fileCriterias.Any(filter => filter(file)));

            if (hasRootFile)
            {
                return currentDirectory;
            }

            return NavigateToSourceRoot(currentDirectory.Parent);
        }
    }
}