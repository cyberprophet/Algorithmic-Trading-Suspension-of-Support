using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ShareInvest.Filter
{
	public class ClientIpCheckActionFilter : ActionFilterAttribute
	{
		public ClientIpCheckActionFilter(string safelist, ILogger logger)
		{
			this.safelist = safelist;
			this.logger = logger;
		}
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var badIp = true;
			var address = context.HttpContext.Connection.RemoteIpAddress;
			logger.LogDebug("Remote IpAddress: {RemoteIp}", address);

			if (address.IsIPv4MappedToIPv6)
				address = address.MapToIPv4();

			foreach (var str in safelist.Split(';'))
			{
				var testIp = IPAddress.Parse(str);

				if (testIp.Equals(address))
				{
					badIp = false;

					break;
				}
			}
			if (badIp)
			{
				logger.LogWarning("Forbidden Request from IP: {RemoteIp}", address);
				context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);

				return;
			}
			base.OnActionExecuting(context);
		}
		readonly ILogger logger;
		readonly string safelist;
	}
}