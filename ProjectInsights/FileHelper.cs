using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInsights
{
    public class FileHelper
    {
        static readonly int stopCount = 45;
        const string space = " ";
        const string CSharpExtention = ".cs";
        static readonly HashSet<string> excludedDirectories = new HashSet<string>() { "Proxies", "Migrations", };

        public static IList<string> GetAuthorsFromFile(StreamReader output, string fileName)
        {
            var list = new List<string>();

            string line;
            while ((line = output.ReadLine()) != null)
            {
                if (line.Contains(fileName))
                {
                    string author = SanitizeLine(line);
                    list.Add(author);
                    //if (author.ToLower().Contains("somename"))
                    //    ben.Add(fileName);
                }
            }

            return list;
        }

        public static string SanitizeLine(string line)
        {
            int startIdx = line.IndexOf("<") + 1;
            int length = line.IndexOf(">") - startIdx;
            line = line.Substring(startIdx, length);
            line = line.Substring(0, line.IndexOf("@")).Replace(".", space);
            return line;
        }

        public static ICollection<string> GetFiles(StreamReader output)
        {
            var files = new HashSet<string>();
            int count = 1;

            string line;
            while ((line = output.ReadLine()) != null)
            {
                if (count == stopCount && stopCount != -1) break;

                AddFile(files, line);
                count++;
            }

            return files;
        }

        public static void AddFile(HashSet<string> files, string line)
        {
            string extension = Path.GetExtension(line);
            bool includedDirectory = IsIncludedDirectory(line);

            if (extension == CSharpExtention && includedDirectory)
            {
                files.Add(line);
            }
        }

        static bool IsIncludedDirectory(string line) => excludedDirectories.Where(a => line.Contains(a)).Count() == 0;

    }
}
