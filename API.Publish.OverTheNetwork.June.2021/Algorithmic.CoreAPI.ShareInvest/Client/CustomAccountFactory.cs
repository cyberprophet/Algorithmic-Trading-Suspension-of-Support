using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace ShareInvest
{
	public class CustomAccountFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
	{
		public CustomAccountFactory(NavigationManager _, IAccessTokenProviderAccessor accessor) : base(accessor)
		{

		}
		public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomUserAccount account, RemoteAuthenticationUserOptions options)
		{
			var initial = await base.CreateUserAsync(account, options);

			if (initial.Identity.IsAuthenticated)
				foreach (var value in account.AuthenticationMethod)
					((ClaimsIdentity)initial.Identity).AddClaim(new Claim("amr", value));

			return initial;
		}
	}
}