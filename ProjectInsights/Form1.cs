using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectInsights
{
    public partial class FormInsights : Form
    {
        private const string Format = "{0,-21}\t : {1,-6}";

        public FormInsights()
        {
            InitializeComponent();
        }

        async void BtnShow_Click(object sender, EventArgs e)
        {
            try
            {
                txtContent.Text = string.Empty;

                Process gitProcess = ProcessHelper.CreateGitProcess("ls-files", txtProjectPath.Text);
                var files = FileHelper.GetFiles(gitProcess.StandardOutput);

                var gitBlameMetrics = await GetGitBlameMetrics(files);
                var gitCommitMetrics = await GetGitCommitMetrics();

                ShowGitBlameMetrics(gitBlameMetrics);
                ShowGitCommitMetrics(gitCommitMetrics);


                //ShowFiles(files);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private async Task<IDictionary<string, (int, int)>> GetGitCommitMetrics()
        {
            string gitLogCommand = "log --pretty=format:\" % ce\" --shortstat";
            var gitProcess = ProcessHelper.CreateGitProcess(gitLogCommand, txtProjectPath.Text);
            var commitMetrics = await MetricsHelper.GetMetricsFromGitLog(gitProcess.StandardOutput);

            EliminateGitCommitDuplicates(commitMetrics);

            return commitMetrics;
        }

        private void EliminateGitCommitDuplicates(Dictionary<string, (int, int)> commitMetrics)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                int similarityPercentage = int.Parse(txtSimilarity.Text);
                combinationFound = MetricsHelper.IsCombinationFound(commitMetrics, similarityPercentage);
            }
        }

        private async Task<IDictionary<string, int>> GetGitBlameMetrics(ICollection<string> files)
        {
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

        private void EliminateGitBlameDuplicates(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                int similarityPercentage = int.Parse(txtSimilarity.Text);
                combinationFound = MetricsHelper.IsCombinationFound(metricsDictionary, similarityPercentage);
            }
        }

        private async Task<IDictionary<string, int>> GetFileMetrics(string fileName)
        {
            string gitBlameCommand = $"blame {fileName} -fte";
            var gitProcess = ProcessHelper.CreateGitProcess(gitBlameCommand, txtProjectPath.Text);
            var authors = await FileHelper.GetAuthorsFromFile(gitProcess.StandardOutput, fileName);
            var fileMetrics = MetricsHelper.GroupMetricsByAuthorName(authors);

            return fileMetrics;
        }



        //private void ShowFiles(ICollection<string> files)
        //{
        //    txtContent.Text += string.Join(Environment.NewLine, files);
        //    txtContent.Text += Environment.NewLine + Environment.NewLine;
        //    txtContent.Text += $"Files Count: {files.Count}";
        //}

        private void ShowGitBlameMetrics(IDictionary<string, int> metrics)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" -- Percentages -- ");
            sb.AppendLine();

            int sum = metrics.Values.Sum();
            foreach (var item in metrics)
            {
                float percentile = (float)item.Value / sum * 100;
                string formatted = string.Format(Format + " %", item.Key, percentile.ToString("F"));
                sb.AppendLine(formatted);

            }

            sb.AppendLine();
            sb.AppendLine(" -- Lines Of Code -- ");
            sb.AppendLine();

            foreach (var item in metrics)
            {
                string formatted = string.Format(Format, item.Key, item.Value);
                sb.AppendLine(formatted);
            }
            sb.AppendLine();
            sb.AppendLine("Total Lines: " + sum);


            txtContent.Text += sb;
        }

        private void ShowGitCommitMetrics(IDictionary<string, (int, int)> gitCommitMetrics)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" -- Commit Counts -- ");
            sb.AppendLine();

            foreach (var item in gitCommitMetrics.OrderByDescending(a => a.Value.Item2))
            {
                (_, int commitCount) = item.Value;
                string formatted = string.Format(Format, item.Key, commitCount);
                sb.AppendLine(formatted);
            }

            sb.AppendLine();
            sb.AppendLine(" -- Average Commit Size -- ");
            sb.AppendLine();

            foreach (var item in gitCommitMetrics.OrderByDescending(a => a.Value.Item1 / a.Value.Item2))
            {
                (int totalChange, int commitCount) = item.Value;
                int averageCommitSize = totalChange / commitCount;
                string formatted = string.Format(Format, item.Key, averageCommitSize);
                sb.AppendLine(formatted);
            }

            sb.AppendLine();
            sb.AppendLine(" -- Total Line of Code Change -- ");
            sb.AppendLine();

            foreach (var item in gitCommitMetrics.OrderByDescending(a => a.Value.Item1))
            {
                (int totalChange, int commitCount) = item.Value;
                string formatted = string.Format(Format, item.Key, totalChange);
                sb.AppendLine(formatted);
            }

            txtContent.Text += sb;
        }

        private void FormInsights_Load(object sender, EventArgs e)
        {
            txtProjectPath.Text = Constants.RepositoryPath;
        }

        private void TxtSimilarity_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }
    }
}
