namespace ShareInvest.Interface.Server
{
	public interface IJwtTokenService
	{
		string BuildToken(string email);
	}
}