using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DirectoryJunctionProject.ViewModel
{
    static class CommandLineHelper
    {
        public async static Task<string> RunCmdAsync(params string[] commands)
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

                cmdInput.Close();
                cmdOutputString = await cmdOutput.ReadToEndAsync();
            }

            return cmdOutputString ?? string.Empty;
        }
    }
}
