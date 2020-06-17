using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInsights
{
    public class GitBlame
    {
        private readonly string projectPath;

        private readonly int similarityPercentage;

        public GitBlame(string prjPath, string similarity)
        {
            projectPath = prjPath;
            similarityPercentage = int.Parse(similarity);
        }
        public async Task<IDictionary<string, int>> GetGitBlameMetrics()
        {
            Process gitProcess = ProcessHelper.CreateGitProcess("ls-files", projectPath);
            var files = FileHelper.GetFiles(gitProcess.StandardOutput);

            var metrics = await CalculateGitBlameMetrics(files);

            EliminateGitBlameDuplicates(metrics);

            var sortedMetrics = GetSortedMetrics(metrics);

            return sortedMetrics;
        }

        private static IDictionary<string, int> GetSortedMetrics(IDictionary<string, int> metrics)
            => metrics.OrderByDescending(a => a.Value).ToDictionary(a => a.Key, b => b.Value);

        private async Task<IDictionary<string, int>> CalculateGitBlameMetrics(ICollection<string> files)
        {
            var metrics = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var fileMetrics = await GetFileMetrics(file);
                MetricsHelper.AddFileMetrics(metrics, fileMetrics);
            }

            return metrics;
        }

        private async Task<IDictionary<string, int>> GetFileMetrics(string fileName)
        {
            string gitBlameCommand = $"blame {fileName} -fte";
            var gitProcess = ProcessHelper.CreateGitProcess(gitBlameCommand, projectPath);
            var authors = await FileHelper.GetAuthorsFromFile(gitProcess.StandardOutput, fileName);
            var fileMetrics = MetricsHelper.GroupMetricsByAuthorName(authors);

            return fileMetrics;

        }

        private void EliminateGitBlameDuplicates(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                combinationFound = MetricsHelper.IsCombinationFound(metricsDictionary, similarityPercentage);
            }
        }


    }
}
