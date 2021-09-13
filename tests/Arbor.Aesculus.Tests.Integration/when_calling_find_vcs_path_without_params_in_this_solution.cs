﻿using System.IO;
using Arbor.Aesculus.Core;
using Machine.Specifications;
using NCrunch.Framework;

namespace Arbor.Aesculus.Tests.Integration
{
    [Subject(typeof(VcsPathHelper))]
    public class when_calling_find_vcs_path_without_params_in_this_solution
    {
        static string? vcsRootPath;

        private Because of = () =>
        {
            if (NCrunchEnvironment.NCrunchIsResident())
            {
                vcsRootPath = VcsPathHelper.FindVcsRootPath(new FileInfo(NCrunchEnvironment.GetOriginalSolutionPath())
                                                           .Directory?.FullName);
            }
            else
            {
                vcsRootPath = Path.GetTempPath();
            }
        };

        It should_not_return_null = () => vcsRootPath.ShouldNotBeNull();

        It should_return_an_existing_directory = () => Directory.Exists(vcsRootPath).ShouldBeTrue();
    }
}