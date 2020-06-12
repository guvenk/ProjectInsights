using System.Collections.Generic;

namespace ProjectInsights
{
    static class Constants
    {
        //readonly char[] nameDelimeters = new[] { ' ', '.', '-' };

        public static readonly int StopCount = 40;
        public const string Space = " ";
        public const string CSharpExtention = ".cs";
        public static readonly HashSet<string> ExcludedDirectories = new HashSet<string>() { "Proxies", "Migrations", };
        public const string RepositoryPath = @"C:\Users\Guven\Desktop\BI";
    }
}
