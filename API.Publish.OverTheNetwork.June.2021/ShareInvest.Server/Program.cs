using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest
{
	class Program
	{
		static void Main()
		{
			foreach (var securtiy in new[] { Security.Commands, Security.Compress })
				ChooseTheInstallationPath(securtiy);

			if (Security.SendUpdateToFile().Result is string file)
				File.Delete(file);
		}
		static void ChooseTheInstallationPath(dynamic param)
		{
			using var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = cmd,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					WorkingDirectory = param.Item1
				}
			};
			if (process.Start())
			{
				process.StandardInput.Write(param.Item2 + Environment.NewLine);
				process.StandardInput.Close();
				Console.WriteLine(process.StandardOutput.ReadToEnd());
				process.WaitForExit();
			}
		}
		const string cmd = @"cmd";
	}
}