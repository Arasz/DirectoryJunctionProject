using System.Diagnostics;
using System.IO;

namespace DirectoryJunctionProject.ViewModel
{
    static class CommandLineHelper
    {
        public static string RunCmd(params string[] commands)
        {
            string cmdOutputString;

            ProcessStartInfo info = new ProcessStartInfo("cmd")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(info))
            using (StreamWriter cmdInput = process.StandardInput)
            using (StreamReader cmdOutput = process.StandardOutput)
            {
                foreach (string command in commands)
                    cmdInput.WriteLine(command);

                cmdOutputString = cmdOutput.ReadToEnd();
            }

            return cmdOutputString ?? string.Empty;
        }
    }
}
