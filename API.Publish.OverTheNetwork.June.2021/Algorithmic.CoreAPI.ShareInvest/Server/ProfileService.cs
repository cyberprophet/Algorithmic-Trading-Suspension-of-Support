using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.Models;
using IdentityServer4.Services;

namespace ShareInvest
{
	public class ProfileService : IProfileService
	{
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			context.IssuedClaims.AddRange(context.Subject.FindAll(JwtClaimTypes.Name));
			context.IssuedClaims.AddRange(context.Subject.FindAll(JwtClaimTypes.Role));
			await Task.CompletedTask;
		}
		public async Task IsActiveAsync(IsActiveContext context) => await Task.CompletedTask;
	}
}