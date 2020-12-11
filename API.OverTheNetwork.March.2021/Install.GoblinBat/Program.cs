using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using ShareInvest.Client;
using ShareInvest.Verify;

namespace ShareInvest
{
	[SupportedOSPlatform("windows")]
	class Program
	{
		static void Main()
		{
			var client = GoblinBat.GetInstance(Security.GetUserInformation(KeyDecoder.ProductKeyFromRegistry));
			var security = new dynamic[] { Security.Commands, Security.Compress, Security.Release };

			for (int i = 0; i < security.Length; i++)
				ChooseTheInstallationPath(i == 1 ? client : null, security[i]).Wait();
		}
		static async Task ChooseTheInstallationPath(dynamic client, dynamic param)
		{
			foreach (var str in param)
			{
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
				if (client is GoblinBat request && await request.PostConfirmAsync(await Security.GetFile(str.Item2)) is not null)
					Base.SendMessage(typeof(GoblinBat), str.Item1 as string, str.Item2 as string);
			}
		}
		const string cmd = @"cmd";
	}
}