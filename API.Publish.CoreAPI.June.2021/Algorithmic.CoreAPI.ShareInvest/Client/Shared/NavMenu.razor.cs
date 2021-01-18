using Microsoft.AspNetCore.Components;

namespace ShareInvest.Shared
{
	public partial class NavMenuBase : ComponentBase
	{
		bool CollapseNavMenu
		{
			get; set;
		}
		public NavMenuBase() => CollapseNavMenu = true;
		protected internal string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;
		protected internal void ToggleNavMenu() => CollapseNavMenu = CollapseNavMenu is false;
	}
}