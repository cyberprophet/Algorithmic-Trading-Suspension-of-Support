using Microsoft.AspNetCore.Components;

namespace MrMind.Shared
{
	public class NavMenuBase : ComponentBase
	{
		public NavMenuBase() => CollapseNavMenu = true;
		protected internal string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;
		protected internal void ToggleNavMenu() => CollapseNavMenu = CollapseNavMenu is false;
		bool CollapseNavMenu
		{
			get; set;
		}
	}
}