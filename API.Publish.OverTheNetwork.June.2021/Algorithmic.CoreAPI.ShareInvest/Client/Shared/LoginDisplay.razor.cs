using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ShareInvest.Shared
{
	public partial class LoginDisplayBase : ComponentBase
	{
		protected internal async Task BeginSignOut(MouseEventArgs _)
		{
			await SignOutManager.SetSignOutState();
			Navigation.NavigateTo("authentication/logout");
		}
		[Inject]
		protected internal NavigationManager Navigation
		{
			get; set;
		}
		[Inject]
		protected internal SignOutSessionStateManager SignOutManager
		{
			get; set;
		}
	}
}