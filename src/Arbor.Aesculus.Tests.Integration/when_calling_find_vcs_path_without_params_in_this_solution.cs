using System.IO;

using Arbor.Aesculus.Core;

using Machine.Specifications;

namespace Arbor.Aesculus.Tests.Integration
{
    [Subject(typeof(VcsPathHelper))]
    public class when_calling_find_vcs_path_without_params_in_this_solution
    {
        static string vcsRootPath;

        Because of = () => { vcsRootPath = VcsPathHelper.FindVcsRootPath(); };

        It should_not_return_null = () => vcsRootPath.ShouldNotBeNull();

        It should_return_an_existing_directory = () => Directory.Exists(vcsRootPath).ShouldBeTrue();
    }
}