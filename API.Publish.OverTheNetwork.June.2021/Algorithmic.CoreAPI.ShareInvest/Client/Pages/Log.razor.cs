using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace ShareInvest.Pages
{
	public partial class LogBase : ComponentBase
	{
		[Inject]
		protected internal HttpClient Http
		{
			get; set;
		}
		protected internal Catalog.Models.Log[] Logs
		{
			get; private set;
		}
		protected override async Task OnInitializedAsync() => Logs = await Http.GetFromJsonAsync<Catalog.Models.Log[]>(Crypto.Security.GetRoute("Message", Security.Identify));
	}
}