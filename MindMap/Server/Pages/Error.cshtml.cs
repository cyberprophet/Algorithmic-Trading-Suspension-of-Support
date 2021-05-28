using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace MrMind.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
	public class ErrorModel : PageModel
	{
		public string RequestId
		{
			get; set;
		}
		public bool ShowRequestId => string.IsNullOrEmpty(RequestId) is false;
		public ErrorModel(ILogger<ErrorModel> logger) => this.logger = logger;
		public void OnGet()
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

			if (string.IsNullOrEmpty(RequestId) is false)
				logger.LogError(HttpContext.TraceIdentifier);
		}
		readonly ILogger<ErrorModel> logger;
	}
}