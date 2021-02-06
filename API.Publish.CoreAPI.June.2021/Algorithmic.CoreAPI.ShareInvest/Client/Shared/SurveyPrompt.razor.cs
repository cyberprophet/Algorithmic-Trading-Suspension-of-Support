using Microsoft.AspNetCore.Components;

namespace ShareInvest.Shared
{
	public class SurveyPromptBase : ComponentBase
	{
		[Parameter]
		public string Title
		{
			get; set;
		}
	}
}