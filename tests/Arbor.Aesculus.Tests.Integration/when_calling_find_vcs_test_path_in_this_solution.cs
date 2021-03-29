using System.IO;
using Arbor.Aesculus.Core;
using Arbor.Aesculus.NCrunch;
using Machine.Specifications;

namespace Arbor.Aesculus.Tests.Integration
{
    [Subject(typeof(VcsPathHelper))]
    public class when_calling_find_vcs_test_path_in_this_solution
    {
        static string vcsRootPath;

        Because of = () => { vcsRootPath = VcsTestPathHelper.TryFindVcsRootPath(); };

        It should_not_return_null = () => vcsRootPath.ShouldNotBeNull();

        It should_return_an_existing_directory = () => Directory.Exists(vcsRootPath).ShouldBeTrue();
    }
}