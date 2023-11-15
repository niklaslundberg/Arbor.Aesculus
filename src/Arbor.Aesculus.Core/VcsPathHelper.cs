using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arbor.Aesculus.Core
{
    public static class VcsPathHelper
    {
        private static readonly List<string> SourceRootDirectoryNames = new() { ".git" };

        private static readonly List<string> SourceRootFileNames = new() { ".deployment", ".gitattributes" };

        private static readonly string RootNotFoundMessage =
            $"Could not find the source root. Searched for folder containing any subfolder with name [{string.Join("|", SourceRootDirectoryNames)}] and searched for folder containing any file with name [{string.Join("|", SourceRootFileNames)}]. None of these were found";

        public static string? TryFindVcsRootPath(string? startDirectory = null, Action<string>? logger = null)
        {
            string directoryPath = !string.IsNullOrWhiteSpace(startDirectory)
                ? startDirectory!
                : AppContext.BaseDirectory;

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The start directory '{directoryPath}' does not exist");
            }

            if (logger != null)
            {
                logger.Invoke($"Using start directory '{startDirectory}'");

                logger.Invoke(
                    $"Searching for folder containing any subfolder with name [{string.Join("|", SourceRootDirectoryNames)}]");

                logger.Invoke(
                    $"Searching for folder containing any file with name [{string.Join("|", SourceRootFileNames)}]");
            }

            var startDir = new DirectoryInfo(directoryPath);

            DirectoryInfo? rootDirectory = NavigateToSourceRoot(startDir);

            if (rootDirectory is null)
            {
                logger?.Invoke(RootNotFoundMessage);
            }

            return rootDirectory?.FullName;
        }

        public static string? FindVcsRootPath(string? startDirectory = null)
        {
            string? rootDirectory = TryFindVcsRootPath(startDirectory);

            if (!string.IsNullOrWhiteSpace(rootDirectory))
            {
                return rootDirectory;
            }

            throw new DirectoryNotFoundException(RootNotFoundMessage);
        }

        private static DirectoryInfo? NavigateToSourceRoot(DirectoryInfo? currentDirectory)
        {
            if (currentDirectory?.Parent is null)
            {
                return null;
            }

            bool IsNamedRoot(DirectoryInfo dir)
            {
                return SourceRootDirectoryNames.Exists(rootName =>
                    dir.Name.Equals(rootName, StringComparison.OrdinalIgnoreCase));
            }

            var directoryCriterion = new List<Func<DirectoryInfo, bool>> { IsNamedRoot };

            bool IsRootFile(FileInfo file)
            {
                return SourceRootFileNames.Exists(rootFile =>
                    file.Name.Equals(rootFile, StringComparison.OrdinalIgnoreCase));
            }

            var fileCriterion = new List<Func<FileInfo, bool>> { IsRootFile };

            IEnumerable<DirectoryInfo> subDirectories = currentDirectory.EnumerateDirectories();

            bool directoryHasSourceRoot = subDirectories.Any(dir =>
                SourceRootDirectoryNames.Exists(pattern => dir.Name.Equals(pattern, StringComparison.OrdinalIgnoreCase)));

            if (directoryHasSourceRoot)
            {
                return currentDirectory;
            }

            bool isRootDir = directoryCriterion.Exists(filter => filter(currentDirectory));

            if (isRootDir)
            {
                return currentDirectory;
            }

            bool hasRootFile = currentDirectory.EnumerateFiles().Any(file => fileCriterion.Exists(filter => filter(file)));

            if (hasRootFile)
            {
                return currentDirectory;
            }

            return NavigateToSourceRoot(currentDirectory.Parent);
        }
    }
}