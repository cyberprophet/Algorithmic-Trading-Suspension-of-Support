using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest
{
	class Program
	{
		static void Main()
		{
			var security = new Security();

			for (int i = 0; i < security.Length + 1; i++)
			{
				if (i < security.Length)
					foreach (var securtiy in new[] { security.Commands(i), security.Compress(i) })
						ChooseTheInstallationPath(securtiy);

				if ((i < security.Length || i == security.Length && Security.Exist) && security.SendUpdateToFile(i).Result is string file && (i == 0 || i == security.Length))
					File.Delete(file);
			}
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