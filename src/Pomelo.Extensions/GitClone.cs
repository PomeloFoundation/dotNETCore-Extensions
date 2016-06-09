using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Pomelo.Package
{
    public class GitCloneResult
    {
        public bool IsSucceeded { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }

    public static class GitClone
    {
        public static GitCloneResult Clone(string uri, string dest, string branch = null, int timeLimit = 1000 * 60 * 20)
        {
            if (!System.IO.Directory.Exists(dest))
                System.IO.Directory.CreateDirectory(dest);
            Process p = new Process();

            var argument = $"clone {uri}";
            if (!string.IsNullOrEmpty(branch))
                argument = $"clone {uri} -b {branch}";

            p.StartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "git",
                Arguments = argument,
                WorkingDirectory = dest
            };
            p.Start();
            if (!p.WaitForExit(timeLimit))
                p.Kill();
            var ret = new GitCloneResult
            {
                IsSucceeded = true,
                Output = p.StandardOutput.ReadToEnd(),
                Error = p.StandardError.ReadToEnd()
            };
            if (p.ExitCode != 0)
                ret.IsSucceeded = false;
            return ret;
        }
    }
}
