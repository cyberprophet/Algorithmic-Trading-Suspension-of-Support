using System;
using System.Diagnostics;

namespace ShareInvest
{
    class Program
    {
        static void Main()
        {
            ChooseTheInstallationPath(Security.Commands);

            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
        static void ChooseTheInstallationPath(dynamic param)
        {
            foreach (var str in param)
                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = cmd,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WorkingDirectory = str.Item1
                    }
                })
                    if (process.Start())
                    {
                        process.StandardInput.Write(str.Item2 + Environment.NewLine);
                        process.StandardInput.Close();
                        Console.WriteLine(process.StandardOutput.ReadToEnd());
                        process.WaitForExit();
                    }
        }
        const string cmd = @"cmd";
    }
}