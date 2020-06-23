using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (string.IsNullOrWhiteSpace(txtProjectPath.Text))
                {
                    lblError.Text = "Please select a project path.";
                    return;
                }

                txtContent.Text = "Please wait...";

                GitBlame gitBlame = new GitBlame(txtProjectPath.Text, txtSimilarity.Text);
                GitCommit gitCommit = new GitCommit(txtProjectPath.Text, txtSimilarity.Text);

                var gitBlameMetrics = gitBlame.GetGitBlameMetrics();
                var gitCommitMetrics = await gitCommit.GetGitCommitMetrics();

                txtContent.Text = "";

                ShowGitBlameMetrics(gitBlameMetrics);
                ShowGitCommitMetrics(gitCommitMetrics);

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

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
            sb.AppendLine();
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
