using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareInvest.Catalog.Models
{
	public class User
	{
		public string[] Id
		{
			get; set;
		}
		public Account Account
		{
			get; set;
		}
		public CancellationToken Token
		{
			get; private set;
		}
		public Queue<Log> Logs
		{
			get; set;
		}
		public WebSocket Socket
		{
			get; private set;
		}
		public Dictionary<string, Balance> Balance
		{
			get; set;
		}
		public async Task AddSocketAsync(WebSocket socket, ArraySegment<byte> seg, WebSocketReceiveResult result, CancellationToken token)
		{
			Socket = socket;
			Token = token;

			while (result.CloseStatus.HasValue is false)
			{
				result = await socket.ReceiveAsync(seg, token);

				if (result.EndOfMessage)
				{
					if (WebSocketMessageType.Text.Equals(result.MessageType) && result.Count > 0)
						Send?.Invoke(this, Encoding.UTF8.GetString(seg.Array, seg.Offset, result.Count));

					else if (WebSocketMessageType.Close.Equals(result.MessageType) && result.Count == 0)
						await socket.CloseOutputAsync(result.CloseStatus.Value, result.CloseStatusDescription, token);
				}
			}
		}
		public void Dispose() => Socket.Dispose();
		public event EventHandler<string> Send;
	}
}