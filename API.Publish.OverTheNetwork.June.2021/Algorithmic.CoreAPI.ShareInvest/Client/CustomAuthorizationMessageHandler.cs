﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ShareInvest
{
	public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
	{
		public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation) : base(provider, navigation) => ConfigureHandler(authorizedUrls: new[] { "https://localhost:44304" }, scopes: new[] { "CoreAPIAPI", "Algorithmic", "Trading" });
	}
}