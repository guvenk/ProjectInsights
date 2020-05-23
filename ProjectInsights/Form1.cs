using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private const string CSharpExtention = ".cs";
        private readonly int stopCount = 45;
        private readonly HashSet<string> excludedDirectories = new HashSet<string>() { "Proxies", "Migrations", };
        //private readonly char[] nameDelimeters = new[] { ' ', '.', '-' };

        private const string projectDirectory = @"C:\Users\Guven\Desktop\BI";
        private const string space = " ";

        private void btnShow_Click(object sender, EventArgs e)
        {
            try
            {
                txtContent.Text = string.Empty;

                Process gitProcess = CreateGitProcess("ls-files");
                var files = GetFiles(gitProcess.StandardOutput);
                var metrics = GetMetrics(files);
                ShowMetrics(metrics);
                //ShowFiles(files);

            }
            catch (Exception ex) { lblError.Text = ex.Message; }
        }

        private IDictionary<string, int> GetMetrics(ICollection<string> files)
        {
            var metrics = CalculateMetrics(files);

            EliminateDuplicates(metrics);

            var sortedMetrics = GetSortedMetrics(metrics);

            return sortedMetrics;
        }

        private static IDictionary<string, int> GetSortedMetrics(IDictionary<string, int> metrics)
        => metrics.OrderByDescending(a => a.Value).ToDictionary(a => a.Key, b => b.Value);


        private IDictionary<string, int> CalculateMetrics(ICollection<string> files)
        {
            var metrics = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var fileMetrics = GetFileMetrics(file);
                AddFileMetrics(metrics, fileMetrics);
            }

            return metrics;
        }


        private void EliminateDuplicates(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = true;

            while (combinationFound)
            {
                combinationFound = IsCombinationFound(metricsDictionary);
            }
        }

        private bool IsCombinationFound(IDictionary<string, int> metricsDictionary)
        {
            bool combinationFound = false;
            foreach (var metric in metricsDictionary)
            {
                string firstName = GetFirstPart(metric.Key);
                var otherKeys = GetOtherKeys(metricsDictionary, metric);
                foreach (var otherKey in otherKeys)
                {
                    combinationFound = ProcessCombination(metricsDictionary, metric, firstName, otherKey);
                    if (combinationFound) break;
                }
                if (combinationFound) break;
            }

            return combinationFound;
        }

        private bool ProcessCombination(IDictionary<string, int> metricsDictionary, KeyValuePair<string, int> metric, string firstName, string otherKey)
        {
            string otherFirstName = GetFirstPart(otherKey);
            int similarityAllowance = int.Parse(txtSimilarity.Text);

            if (StringHelper.GetSimilarityPercentage(otherFirstName, firstName) >= similarityAllowance)
            {
                CombineAuthors(metricsDictionary, metric.Key, otherKey);
                return true;
            }

            return false;
        }

        private static IEnumerable<string> GetOtherKeys(IDictionary<string, int> metricsDictionary, KeyValuePair<string, int> metric)
        {
            return metricsDictionary.Keys.Where(a => a != metric.Key);
        }

        private static string GetFirstPart(string key) => key.Split(space).First();


        private static void CombineAuthors(IDictionary<string, int> dict, string metricKey, string otherKey)
        {
            int total = dict[metricKey] + dict[otherKey];

            if (metricKey.Length > otherKey.Length)
            {
                dict[metricKey] = total;
                dict.Remove(otherKey);
            }
            else
            {
                dict[otherKey] = total;
                dict.Remove(metricKey);
            }
        }

        private IDictionary<string, int> GetFileMetrics(string fileName)
        {
            string gitBlameCommand = $"blame {fileName} -fte";
            var gitProcess = CreateGitProcess(gitBlameCommand);
            var authors = GetAuthorsFromFile(gitProcess.StandardOutput, fileName);
            var fileMetrics = GroupMetricsByAuthorName(authors);

            return fileMetrics;
        }

        private static Dictionary<string, int> GroupMetricsByAuthorName(IList<string> authors)
        {
            return authors.GroupBy(a => a).ToDictionary(g => g.Key, g => g.Count());
        }

        private static void AddFileMetrics(IDictionary<string, int> metrics, IDictionary<string, int> fileMetrics)
        {
            foreach (var pair in fileMetrics)
            {
                if (!metrics.ContainsKey(pair.Key))
                    metrics[pair.Key] = pair.Value;
                else
                    metrics[pair.Key] += pair.Value;
            }
        }

        private IList<string> GetAuthorsFromFile(StreamReader output, string fileName)
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

        private static string SanitizeLine(string line)
        {
            int startIdx = line.IndexOf("<") + 1;
            int length = line.IndexOf(">") - startIdx;
            line = line.Substring(startIdx, length);
            line = line.Substring(0, line.IndexOf("@")).Replace(".", space);
            return line;
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

        private ICollection<string> GetFiles(StreamReader output)
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

        private void AddFile(HashSet<string> files, string line)
        {
            string extension = Path.GetExtension(line);
            bool includedDirectory = IsIncludedDirectory(line);

            if (extension == CSharpExtention && includedDirectory)
            {
                files.Add(line);
            }
        }

        private bool IsIncludedDirectory(string line) => excludedDirectories.Where(a => line.Contains(a)).Count() == 0;


        private void ShowFiles(ICollection<string> files)
        {
            txtContent.Text += string.Join(Environment.NewLine, files);
            txtContent.Text += Environment.NewLine + Environment.NewLine;
            txtContent.Text += $"Files Count: {files.Count}";
        }

        private void ShowMetrics(IDictionary<string, int> metrics)
        {
            var sb = new StringBuilder();
            foreach (var metric in metrics)
            {
                string formatted = string.Format("{0,-20}\t : {1,-6}", metric.Key, metric.Value);
                sb.AppendLine(formatted);
            }
            txtContent.Text += sb;
        }

        private void FormInsights_Load(object sender, EventArgs e)
        {
            txtProjectPath.Text = projectDirectory;
        }

        private void txtSimilarity_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
                e.Handled = true;
        }
    }
}
