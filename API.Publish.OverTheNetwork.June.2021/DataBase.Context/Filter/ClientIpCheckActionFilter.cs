using Microsoft.AspNetCore.Http;
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
			var address = context.HttpContext.Request.Host.Host;
			logger.LogDebug("Remote IpAddress: {RemoteIp}", address);

			foreach (var str in safelist.Split(';'))
			{
				if (address.Equals(str))
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