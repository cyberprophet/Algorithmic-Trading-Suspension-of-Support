using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using ShareInvest.Interface.Server;

namespace ShareInvest.Service
{
	public class JwtTokenService : IJwtTokenService
	{
		public string BuildToken(string email)
		{
			if (double.TryParse(config["Jwt:ExpireTime"], out double time))
				return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], new[] { new Claim(JwtRegisteredClaimNames.Email, email), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) }, expires: DateTime.Now.AddMinutes(time), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])), SecurityAlgorithms.HmacSha256)));

			else
				return null;
		}
		public JwtTokenService(IConfiguration config) => this.config = config;
		readonly IConfiguration config;
	}
}