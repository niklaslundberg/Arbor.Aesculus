using System;
using System.Linq;
using Arbor.Aesculus.Core;

namespace Arbor.Aesculus.ConsoleApp
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var result = VcsPathHelper.TryFindVcsRootPath(args.FirstOrDefault());

            if (string.IsNullOrWhiteSpace(result))
            {
                return -1;
            }

            Console.WriteLine(result);

            return 0;
        }
    }
}