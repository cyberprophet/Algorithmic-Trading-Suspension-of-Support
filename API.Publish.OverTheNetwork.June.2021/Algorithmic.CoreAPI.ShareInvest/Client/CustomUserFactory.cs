using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace ShareInvest
{
	public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
	{
		public CustomUserFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
		{

		}
		public async override ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
		{
			var user = await base.CreateUserAsync(account, options);

			if (user.Identity.IsAuthenticated)
			{
				var identity = (ClaimsIdentity)user.Identity;
				var roleClaims = identity.FindAll(identity.RoleClaimType).ToArray();

				if (roleClaims != null && roleClaims.Any())
				{
					foreach (var existingClaim in roleClaims)
						identity.RemoveClaim(existingClaim);

					if (account.AdditionalProperties[identity.RoleClaimType] is JsonElement roles)
					{
						if (roles.ValueKind == JsonValueKind.Array)
							foreach (var role in roles.EnumerateArray())
								identity.AddClaim(new Claim(options.RoleClaim, role.GetString()));

						else
							identity.AddClaim(new Claim(options.RoleClaim, roles.GetString()));
					}
				}
			}
			return user;
		}
	}
}