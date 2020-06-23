using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

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

        public IDictionary<string, int> GetGitBlameMetrics()
        {
            var metrics = new Dictionary<string, int>();
            using (var repo = new Repository(projectPath))
            {
                var files = repo.Index
                    .Where(x => FileHelper.IsValidFile(x.Path))
                    .Select(x => x.Path)
                    .ToList();

                foreach (var file in files)
                {
                    var blame = repo.Blame(file);
                    int lineCount = 0;
                    foreach (var item in blame)
                    {
                        string authorMail = StringHelper.SanitizeEmail(item.InitialSignature.Email);
                        if (!metrics.ContainsKey(authorMail))
                            metrics[authorMail] = item.LineCount;
                        else
                            metrics[authorMail] += item.LineCount;

                        lineCount += item.LineCount;
                    }
                }
            }

            EliminateGitBlameDuplicates(metrics);

            var sortedMetrics = GetSortedMetrics(metrics);

            return sortedMetrics;
        }

        private static IDictionary<string, int> GetSortedMetrics(IDictionary<string, int> metrics)
        {
            return metrics.OrderByDescending(a => a.Value).ToDictionary(a => a.Key, b => b.Value);
        }

        private void EliminateGitBlameDuplicates(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = true;

            while (combinationFound)
                combinationFound = MetricsHelper.IsCombinationFound(metricsDictionary, similarityPercentage);
        }
    }
}
