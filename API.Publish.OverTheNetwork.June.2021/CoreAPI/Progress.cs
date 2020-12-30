using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

using ShareInvest.Catalog.Models;
using ShareInvest.Client;

namespace ShareInvest
{
	static class Progress
	{
		internal static API API
		{
			get; set;
		}
		internal static Dictionary<string, Queue<Collect>> Collection
		{
			get; private set;
		}
		internal static void TryToConnectThePipeStream(dynamic param)
		{
			Collection = new Dictionary<string, Queue<Collect>>(0x800);
			var client = new NamedPipeClientStream(".", API.GetType().Name, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
			new Task(async () =>
			{
				await client.ConnectAsync();
				(param as Security).SendMessage(Pipe.TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected));

				if (client.IsConnected)
					Pipe.OnReceivePipeClientMessage(param, client);

			}).Start();
		}
		internal static async Task<bool> GetUpdateVisionAsync()
		{
			var path = @"C:\Algorithmic Trading\Res\Update\";
			var file = new string[] { "x64.zip", "x86.zip" };
			var exist = false;

			foreach (var name in file)
			{
				var full = string.Concat(path, name);

				if (await API.PostConfirmAsync(new Files
				{
					Path = path,
					Name = name,
					LastWriteTime = new FileInfo(full).LastWriteTime.AddHours(1),
					Contents = null
				}) is Files neo)
				{
					using (var stream = new FileStream(full, FileMode.Create))
						await stream.WriteAsync(neo.Contents.AsMemory(0, neo.Contents.Length));

					exist = exist || neo.Contents is not null && neo.Contents.Length > 0;
				}
			}
			return exist;
		}
	}
}