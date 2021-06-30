using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;

namespace ShareInvest.Filter
{
	public class RequireHttpsConnectionFilter : RequireHttpsAttribute
	{
		public override void OnAuthorization(AuthorizationFilterContext context)
		{
			if (context is null)
				throw new ArgumentNullException(nameof(context));

			if (context.HttpContext.Request.IsHttps)
				return;

			base.OnAuthorization(context);
		}
	}
}