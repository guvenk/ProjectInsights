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
        private readonly int stopCount = 15;
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
                HashSet<string> files = GetFiles(gitProcess.StandardOutput);
                var metrics = GetMetrics(files);
                ShowMetrics(metrics);
                //ShowFiles(files);

            }
            catch (Exception ex) { lblError.Text = ex.Message; }
        }

        private Dictionary<string, int> GetMetrics(HashSet<string> files)
        {
            var metrics = CalculateMetrics(files);

            EliminateDuplicates(metrics);

            var sortedMetrics = metrics.OrderByDescending(a => a.Value).ToDictionary(a => a.Key, b => b.Value);

            return sortedMetrics;
        }

        private void EliminateDuplicates(Dictionary<string, int> metricsDictionary)
        {
            var dic = new Dictionary<string, int>();
            bool combinationFound = true;

            while (combinationFound)
            {
                combinationFound = false;
                foreach (var metric in metricsDictionary)
                {
                    string firstName = metric.Key.Split(space).First();
                    foreach (var otherKey in metricsDictionary.Keys.Where(a => a != metric.Key))
                    {
                        string otherFirstName = otherKey.Split(space).First();
                        int similarityAllowance = int.Parse(txtSimilarity.Text);

                        if (StringHelper.GetSimilarityPercentage(otherFirstName, firstName) >= similarityAllowance)
                        {
                            int total = metricsDictionary[metric.Key] + metricsDictionary[otherKey];

                            if (metric.Key.Length > otherKey.Length)
                            {
                                metricsDictionary[metric.Key] = total;
                                metricsDictionary.Remove(otherKey);
                            }
                            else
                            {
                                metricsDictionary[otherKey] = total;
                                metricsDictionary.Remove(metric.Key);
                            }
                            combinationFound = true;
                            break;
                        }
                    }
                    if (combinationFound) break;
                }
            }
        }

        private Dictionary<string, int> CalculateMetrics(HashSet<string> files)
        {
            var metrics = new Dictionary<string, int>();
            foreach (var file in files)
            {
                var fileMetrics = GetFileMetrics(file);
                AddFileMetrics(metrics, fileMetrics);
            }

            return metrics;
        }

        private static void PrintOutput()
        {
            var sw = new StreamWriter(@"C:\Users\Guven\Desktop\out.txt");
            foreach (var item in output)
                sw.WriteLine(item);
            sw.Close();
        }

        private Dictionary<string, int> GetFileMetrics(string fileName)
        {
            string gitBlameCommand = $"blame {fileName} -fte";
            var gitProcess = CreateGitProcess(gitBlameCommand);
            var authors = GetAuthorsFromFile(gitProcess.StandardOutput, fileName);
            var fileMetrics = authors.GroupBy(a => a)
                .ToDictionary(g => g.Key, g => g.Count());

            return fileMetrics;
        }

        private static void AddFileMetrics(Dictionary<string, int> metrics, Dictionary<string, int> fileMetrics)
        {
            foreach (var pair in fileMetrics)
            {
                if (!metrics.ContainsKey(pair.Key))
                    metrics[pair.Key] = pair.Value;
                else
                    metrics[pair.Key] += pair.Value;
            }
        }
        static readonly List<string> output = new List<string>();

        private List<string> GetAuthorsFromFile(StreamReader output, string fileName)
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
            line = line.Substring(0, line.IndexOf("@"));
            line = line.Replace(".", space);
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

        private HashSet<string> GetFiles(StreamReader output)
        {
            var files = new HashSet<string>();
            int count = 1;

            string line;
            while ((line = output.ReadLine()) != null)
            {
                if (count == stopCount && stopCount != -1) break;

                string extension = Path.GetExtension(line);
                bool includedDirectory = excludedDirectories.Where(a => line.Contains(a)).Count() == 0;
                if (extension == CSharpExtention && includedDirectory)
                {
                    files.Add(line);
                    count++;
                }
            }
            return files;
        }

        private void ShowFiles(HashSet<string> files)
        {
            txtContent.Text += string.Join(Environment.NewLine, files);
            txtContent.Text += Environment.NewLine + Environment.NewLine;
            txtContent.Text += $"Files Count: {files.Count}";
        }

        private void ShowMetrics(Dictionary<string, int> metrics)
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
