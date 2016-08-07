﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arbor.Aesculus.Core
{
    public static class VcsPathHelper
    {
        private static readonly List<string> _SourceRootDirectoryNames = new List<string> { ".git" };

        private static readonly List<string> _SourceRootFileNames = new List<string> { ".deployment", ".gitattributes" };

        private static string RootNotFoundMessage
            =>
                $"Could not find the source root. Searched for folder containing any subfolder with name [{string.Join("|", _SourceRootDirectoryNames)}] and searched for folder containing any file with name [{string.Join("|", _SourceRootFileNames)}]. None of these were found"
            ;

        public static string TryFindVcsRootPath(string startDirectory = null, bool loggingEnabled = false)
        {
            string directoryPath = !string.IsNullOrWhiteSpace(startDirectory)
                                       ? startDirectory
                                       : AppContext.BaseDirectory;

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The start directory '{directoryPath}' does not exist");
            }

            if (loggingEnabled)
            {
                Console.WriteLine("Using start directory '{0}'", startDirectory);

                Console.WriteLine(
                    "Searching for folder containing any subfolder with name [{0}]",
                    string.Join("|", _SourceRootDirectoryNames));
                Console.WriteLine(
                    "Searching for folder containing any file with name [{0}]",
                    string.Join("|", _SourceRootFileNames));
            }

            var startDir = new DirectoryInfo(directoryPath);

            DirectoryInfo rootDirectory = NavigateToSourceRoot(startDir);

            if (rootDirectory == null && loggingEnabled)
            {
                Console.Error.WriteLine(RootNotFoundMessage);
            }

            return rootDirectory?.FullName;
        }

        public static string FindVcsRootPath(string startDirectory = null)
        {
            var rootDirectory = TryFindVcsRootPath(startDirectory);

            if (!string.IsNullOrWhiteSpace(rootDirectory))
            {
                return rootDirectory;
            }

            throw new DirectoryNotFoundException(RootNotFoundMessage);
        }

        private static DirectoryInfo NavigateToSourceRoot(DirectoryInfo currentDirectory)
        {
            if (currentDirectory?.Parent == null)
            {
                return null;
            }

            Func<DirectoryInfo, bool> isNamedRoot =
                dir =>
                _SourceRootDirectoryNames.Any(
                    rootName => dir.Name.Equals(rootName, StringComparison.OrdinalIgnoreCase));

            var directoryCriterias = new List<Func<DirectoryInfo, bool>> { isNamedRoot };

            Func<FileInfo, bool> isRootFile =
                file =>
                _SourceRootFileNames.Any(
                    rootFile => file.Name.Equals(rootFile, StringComparison.OrdinalIgnoreCase));

            var fileCriterias = new List<Func<FileInfo, bool>> { isRootFile };

            IEnumerable<DirectoryInfo> subDirectories = currentDirectory.EnumerateDirectories();

            bool directoryHasSourceRoot =
                subDirectories.Any(
                    dir =>
                    _SourceRootDirectoryNames.Any(
                        pattern => dir.Name.Equals(pattern, StringComparison.OrdinalIgnoreCase)));
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