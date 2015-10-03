using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DirectoryJunctionProject.ViewModel
{
    static class CommandLineHelper
    {
        public async static Task<string> RunCmdAsync(string command)
        {
            string cmdOutputString;

            ProcessStartInfo info = new ProcessStartInfo("cmd.exe","/c"+command)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(info))
            using (StreamReader cmdOutput = process.StandardOutput)
            {
                cmdOutputString = await cmdOutput.ReadToEndAsync();
            }

            return cmdOutputString ?? string.Empty;
        }
    }
}
