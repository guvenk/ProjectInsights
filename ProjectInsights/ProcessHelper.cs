using System.Diagnostics;

namespace ProjectInsights
{
    public class ProcessHelper
    {
        public static Process CreateGitProcess(string command, string prjAbsolutePath)
        {
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

    }
}
