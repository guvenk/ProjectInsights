using System.Collections.Generic;

using System.Threading.Tasks;

namespace ProjectInsights
{
    public class GitCommit
    {
        private readonly string projectPath;

        private readonly int similarityPercentage;

        public GitCommit(string prjPath, string similarity)
        {
            projectPath = prjPath;
            similarityPercentage = int.Parse(similarity);
        }
        public async Task<IDictionary<string, (int, int)>> GetGitCommitMetrics()
        {
            string gitLogCommand = "log --pretty=format:\" % ce\" --shortstat";
            var gitProcess = ProcessHelper.CreateGitProcess(gitLogCommand, projectPath);
            var commitMetrics = await MetricsHelper.GetMetricsFromGitLog(gitProcess.StandardOutput);

            EliminateGitCommitDuplicates(commitMetrics);

            return commitMetrics;
        }

        private void EliminateGitCommitDuplicates(Dictionary<string, (int, int)> commitMetrics)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                combinationFound = MetricsHelper.IsCombinationFound(commitMetrics, similarityPercentage);
            }
        }

    }
}
