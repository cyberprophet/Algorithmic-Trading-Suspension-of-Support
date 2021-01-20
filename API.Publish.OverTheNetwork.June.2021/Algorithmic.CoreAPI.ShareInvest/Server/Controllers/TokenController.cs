using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using ShareInvest.Catalog.Models;
using ShareInvest.Interface.Server;
using ShareInvest.Models;

namespace ShareInvest.Controllers
{
	[ApiController, Route(Security.route), Produces(Security.produces)]
	public class TokenController : ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> SetRegistry([FromBody] Token param)
		{
			if (ModelState.IsValid)
			{
				var user = await manager.FindByEmailAsync(param.Email);

				if (string.IsNullOrEmpty(user.Id) is false && await manager.CheckPasswordAsync(user, param.Password))
					return Ok(new
					{
						token = GenerateToken(param.Email)
					});
				if (string.IsNullOrEmpty(user.Id) && (await manager.CreateAsync(new CoreUser
				{
					UserName = param.Email,
					Email = param.Email

				}, param.Password)).Succeeded)
					return Ok(new
					{
						token = GenerateToken(param.Email)
					});
			}
			return BadRequest();
		}
		public TokenController(IJwtTokenService service, UserManager<CoreUser> manager)
		{
			this.service = service;
			this.manager = manager;
		}
		string GenerateToken(string param) => service.BuildToken(param);
		readonly UserManager<CoreUser> manager;
		readonly IJwtTokenService service;
	}
}