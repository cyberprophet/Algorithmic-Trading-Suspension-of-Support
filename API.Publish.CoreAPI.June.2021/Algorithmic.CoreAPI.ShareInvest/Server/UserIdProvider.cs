using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest
{
	public class UserIdProvider : IUserIdProvider
	{
		public virtual string GetUserId(HubConnectionContext connection) => connection.User?.FindFirst(ClaimTypes.Email)?.Value;
	}
}