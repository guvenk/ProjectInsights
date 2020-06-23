using System.IO;
using System.Linq;

namespace ProjectInsights
{
    public class FileHelper
    {
        public static bool IsValidFile(string line)
        {
            string extension = Path.GetExtension(line);
            bool includedDirectory = IsIncludedDirectory(line);

            if (extension == Constants.CSharpExtention && includedDirectory)
                return true;
            else
                return false;
        }

        private static bool IsIncludedDirectory(string line)
        {
            return Constants.ExcludedDirectories.Where(a => line.Contains(a)).Count() == 0;
        }
    }
}
