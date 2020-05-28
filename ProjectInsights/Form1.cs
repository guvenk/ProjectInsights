using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectInsights
{
    public partial class FormInsights : Form
    {
        public FormInsights()
        {
            InitializeComponent();
        }

        void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                txtContent.Text = string.Empty;

                Process gitProcess = CreateGitProcess("ls-files");
                var files = FileHelper.GetFiles(gitProcess.StandardOutput);
                var metrics = GetMetrics(files);
                ShowMetrics(metrics);
                //ShowFiles(files);

            }
            catch (Exception ex) { lblError.Text = ex.Message; }
        }

        IDictionary<string, int> GetMetrics(ICollection<string> files)
        {
            var metrics = CalculateMetrics(files);

            EliminateDuplicates(metrics);

            var sortedMetrics = GetSortedMetrics(metrics);

            return sortedMetrics;
        }

        static IDictionary<string, int> GetSortedMetrics(IDictionary<string, int> metrics)
            => metrics.OrderByDescending(a => a.Value).ToDictionary(a => a.Key, b => b.Value);

        IDictionary<string, int> CalculateMetrics(ICollection<string> files)
        {
            var metrics = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var fileMetrics = GetFileMetrics(file);
                MetricsHelper.AddFileMetrics(metrics, fileMetrics);
            }

            return metrics;
        }

        void EliminateDuplicates(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                int similarityPercentage = int.Parse(txtSimilarity.Text);
                combinationFound = MetricsHelper.IsCombinationFound(metricsDictionary, similarityPercentage);
            }
        }

        IDictionary<string, int> GetFileMetrics(string fileName)
        {
            string gitBlameCommand = $"blame {fileName} -fte";
            var gitProcess = CreateGitProcess(gitBlameCommand);
            var authors = FileHelper.GetAuthorsFromFile(gitProcess.StandardOutput, fileName);
            var fileMetrics = MetricsHelper.GroupMetricsByAuthorName(authors);

            return fileMetrics;
        }

        private Process CreateGitProcess(string command)
        {
            string prjAbsolutePath = txtProjectPath.Text;
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "git",
                WorkingDirectory = prjAbsolutePath,
                CreateNoWindow = true,
                Arguments = $@"--git-dir {prjAbsolutePath}\.git {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process cmdProcess = Process.Start(startInfo);
            return cmdProcess;
        }

        //private void ShowFiles(ICollection<string> files)
        //{
        //    txtContent.Text += string.Join(Environment.NewLine, files);
        //    txtContent.Text += Environment.NewLine + Environment.NewLine;
        //    txtContent.Text += $"Files Count: {files.Count}";
        //}

        private void ShowMetrics(IDictionary<string, int> metrics)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" -- Percentages -- ");
            sb.AppendLine();

            int sum = metrics.Values.Sum();
            foreach (var item in metrics)
            {
                float percentile = (float)item.Value / sum * 100;
                string formatted = string.Format("{0,-20}\t : {1,-6}%", item.Key, percentile.ToString("F"));
                sb.AppendLine(formatted);

            }

            sb.AppendLine();
            sb.AppendLine(" -- Lines Of Code -- ");
            sb.AppendLine();

            foreach (var item in metrics)
            {
                string formatted = string.Format("{0,-20}\t : {1,-6}", item.Key, item.Value);
                sb.AppendLine(formatted);
            }
            sb.AppendLine();
            sb.AppendLine("Total Lines: " + sum);


            txtContent.Text += sb;
        }

        private void FormInsights_Load(object sender, EventArgs e)
        {
            txtProjectPath.Text = Constants.RepositoryPath;
        }

        private void txtSimilarity_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }
    }
}
