using System.Diagnostics;
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
        private const string directory = @"C:\Users\Guven\Desktop\BI";
        private void btnShow_Click(object sender, System.EventArgs e)
        {
            var sb = new StringBuilder();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "git",
                WorkingDirectory = directory,
                CreateNoWindow = true,
                Arguments = @"--git-dir C:\Users\Guven\Desktop\BI\.git ls-files",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process cmdProcess = Process.Start(startInfo);
            string line;
            int count = 0;
            while ((line = cmdProcess.StandardOutput.ReadLine()) != null)
            {
                sb.Append("\r\n" + line);
                count++;
            }
            txtContent.Text += sb;

            txtContent.Text += "\r\n\r\nFiles Count: " + count;
        }
    }
}
