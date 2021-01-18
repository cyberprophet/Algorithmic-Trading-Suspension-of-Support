using System.Collections.Generic;

using IdentityServer4;
using IdentityServer4.Models;

namespace ShareInvest
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource> { new IdentityResources.OpenId(), new IdentityResources.Profile(), };
		public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> { new ApiScope("Trading", "Algorithmic Trading") };
		public static IEnumerable<Client> Clients => new List<Client>
		{
			new Client
			{
				ClientId = "CoreAPI",
				ClientSecrets = { new Secret("secret".Sha256()) },
				AllowedGrantTypes = GrantTypes.ClientCredentials,
				AllowedScopes = { "Trading" }
			},
			new Client
			{
				ClientId = "CoreAPI Client",
				ClientSecrets = { new Secret("secret".Sha256()) },
				AllowedGrantTypes = GrantTypes.Code,
				RedirectUris = { "https://localhost:44304/signin-oidc" },
				PostLogoutRedirectUris = { "https://localhost:44304/signout-callback-oidc" },
				AllowedScopes = new List<string>
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					"Trading"
				}
			}
		};
	}
}