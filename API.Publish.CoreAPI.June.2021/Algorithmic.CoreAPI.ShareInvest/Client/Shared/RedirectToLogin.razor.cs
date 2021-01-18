using System;

using Microsoft.AspNetCore.Components;

namespace ShareInvest.Shared
{
	public partial class RedirectToLoginBase : ComponentBase
	{
		[Inject]
		protected internal NavigationManager Navigation
		{
			get; set;
		}
		protected override void OnInitialized() => Navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Navigation.Uri)}");
	}
}