using Microsoft.AspNetCore.Components;

namespace MrMind.Shared
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