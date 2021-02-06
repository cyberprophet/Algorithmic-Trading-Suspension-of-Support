using Microsoft.AspNetCore.Components;

namespace ShareInvest.Pages
{
	public partial class AuthenticationBase : ComponentBase
	{
		[Parameter]
		public string Action
		{
			get; set;
		}
		[Inject]
		protected internal NavigationManager Manager
		{
			get; set;
		}
		protected internal bool MakeSureLoggedOut => logged_out.Equals(Action);
		const string logged_out = "logged-out";
	}
}