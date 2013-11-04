﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arbor.Aesculus.Core
{
    public static class VcsPathHelper
    {
        static readonly List<string> SourceRootDirectoryNames = new List<string> {".git", ".hg"};
        static readonly List<string> SourceRootFileNames = new List<string> {".deployment", ".gitattributes"};

        static string RootNotFoundMessage
        {
            get
            {
                return string.Format(
                    "Could not find the source root. Searched for folder containing any subfolder with name [{0}] and searched for folder containing any file with name [{1}]. None of these were found",
                    string.Join("|", SourceRootDirectoryNames), string.Join("|", SourceRootFileNames));
            }
        }

        public static string TryFindVcsRootPath(string startDirectory = null, bool logg = false)
        {
            var directoryPath = !string.IsNullOrWhiteSpace(startDirectory)
                                    ? startDirectory
                                    : AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException(string.Format("The start directory '{0}' does not exist",
                                                                   directoryPath));
            }

            if (logg)
            {
                Console.WriteLine("Using start directory '{0}'", startDirectory);

                Console.WriteLine("Searching for folder containing any subfolder with name [{0}]",
                                  string.Join("|", SourceRootDirectoryNames));
                Console.WriteLine("Searching for folder containing any file with name [{0}]",
                                  string.Join("|", SourceRootFileNames));
            }

            var startDir = new DirectoryInfo(directoryPath);

            DirectoryInfo rootDirectory = NavigateToSourceRoot(startDir);

            if (rootDirectory == null)
            {
                Console.Error.WriteLine(RootNotFoundMessage);
            }

            return rootDirectory == null ? null : rootDirectory.FullName;
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

        static DirectoryInfo NavigateToSourceRoot(DirectoryInfo currentDirectory)
        {
            if (currentDirectory.Parent == null)
            {
                return null;
            }

            Func<DirectoryInfo, bool> isNamedRoot =
                dir =>
                SourceRootDirectoryNames.Any(rootName =>
                                             dir.Name.Equals(rootName, StringComparison.InvariantCultureIgnoreCase));

            var directoryCriterias = new List<Func<DirectoryInfo, bool>>
                                         {
                                             isNamedRoot
                                         };

            Func<FileInfo, bool> isRootFile =
                file =>
                SourceRootFileNames.Any(rootFile =>
                                        file.Name.Equals(rootFile, StringComparison.InvariantCultureIgnoreCase));

            var fileCriterias = new List<Func<FileInfo, bool>>
                                    {
                                        isRootFile
                                    };


            var subDirectories = currentDirectory.EnumerateDirectories();
            bool directoryHasSourceRoot = subDirectories.Any(dir =>
                                                             SourceRootDirectoryNames.Any(pattern =>
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