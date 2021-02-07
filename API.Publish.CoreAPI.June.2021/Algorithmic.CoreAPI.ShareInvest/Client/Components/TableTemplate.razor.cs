using System.Collections.Generic;

using Microsoft.AspNetCore.Components;

namespace ShareInvest.Components
{
	public class TableTemplateBase<TItem> : ComponentBase
	{
		[Parameter]
		public RenderFragment TableHeader
		{
			get; set;
		}
		[Parameter]
		public RenderFragment<TItem> RowTemplate
		{
			get; set;
		}
		[Parameter]
		public IReadOnlyList<TItem> Items
		{
			get; set;
		}
	}
}