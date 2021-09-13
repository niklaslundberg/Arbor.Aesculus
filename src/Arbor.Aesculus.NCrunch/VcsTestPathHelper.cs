using System;
using System.IO;
using Arbor.Aesculus.Core;
using NCrunch.Framework;

namespace Arbor.Aesculus.NCrunch
{
    public static class VcsTestPathHelper
    {
        public static string? TryFindVcsRootPath(Action<string>? logger = null)
        {
            if (NCrunchEnvironment.NCrunchIsResident())
            {
                var originalSolutionFileInfo = new FileInfo(NCrunchEnvironment.GetOriginalSolutionPath());
                return VcsPathHelper.TryFindVcsRootPath(originalSolutionFileInfo.Directory?.FullName, logger);
            }

            return VcsPathHelper.TryFindVcsRootPath(logger: logger);
        }
    }
}