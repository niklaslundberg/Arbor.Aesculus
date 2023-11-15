using System.IO;
using Arbor.Aesculus.Core;
using Machine.Specifications;

namespace Arbor.Aesculus.Tests.Integration;

[Subject(typeof(VcsPathHelper))]
public class when_calling_try_find_vcs_path_in_a_non_vcs_directory
{
    static string? vcsRootPath;

    Because of = () => { vcsRootPath = VcsPathHelper.TryFindVcsRootPath(Path.GetTempPath()); };

    It should_return_null = () => vcsRootPath.ShouldBeNull();
}