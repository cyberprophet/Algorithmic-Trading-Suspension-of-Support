using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ShareInvest.Controllers
{
	public class OidcConfigurationController : Controller
	{
		public OidcConfigurationController(IClientRequestParametersProvider client, ILogger<OidcConfigurationController> logger)
		{
			Client = client;
			this.logger = logger;
		}
		public IClientRequestParametersProvider Client
		{
			get;
		}
		[HttpGet(Security.log)]
		public IActionResult GetClientRequestParameters([FromRoute] string clientId)
		{
			if (string.IsNullOrEmpty(clientId))
				logger.LogError(HttpContext.TraceIdentifier);

			return Ok(Client.GetClientParameters(HttpContext, clientId));
		}
		readonly ILogger<OidcConfigurationController> logger;
	}
}