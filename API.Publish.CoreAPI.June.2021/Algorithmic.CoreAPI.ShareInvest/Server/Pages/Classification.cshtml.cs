using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace ShareInvest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
	public class ClassificationModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
		{
			string query = Request.Query["json"];
			var datum = new Dictionary<string, int>();

			if (string.IsNullOrEmpty(query))
			{
				for (int i = 0; i < 0xA; i++)
					if (Security.Conditions is HashSet<string>[] && Security.Conditions[i] is HashSet<string>)
						foreach (var code in Security.Conditions[i])
							foreach (var index in from o in context.Classifications.AsNoTracking() where o.Code.Equals(code) select o.Index)
							{
								var name = context.Theme.AsNoTracking().Single(o => o.Index.Equals(index)).Name;

								if (datum.TryGetValue(name, out int count))
									datum[name] = ++count;

								else
									datum[name] = 1;
							}
				if (datum.Count > 0)
				{
					Statistics = new Dictionary<string, int>();

					foreach (var kv in datum.OrderByDescending(o => o.Value))
					{
						if (string.IsNullOrEmpty(Title))
							Title = kv.Key;

						Statistics[kv.Key] = kv.Value;
					}
					Datum = JsonConvert.SerializeObject(Statistics);

					return Page();
				}
			}
			else
			{
				Statistics = JsonConvert.DeserializeObject<Dictionary<string, int>>(query);

				if (Statistics is Dictionary<string, int> && Statistics.Count > 0)
				{
					foreach (var kv in Statistics.OrderByDescending(o => o.Value))
						if (await context.Theme.AsNoTracking().AnyAsync(o => o.Index.Equals(kv.Key)))
						{
							var name = context.Theme.AsNoTracking().Single(o => o.Index.Equals(kv.Key)).Name;
							datum[name] = kv.Value;

							if (string.IsNullOrEmpty(Title))
								Title = name;
						}
					Datum = JsonConvert.SerializeObject(datum);

					return Page();
				}
				else
					return Unauthorized();
			}
			return BadRequest();
		}
		public ClassificationModel(CoreApiDbContext context) => this.context = context;
		public Dictionary<string, int> Statistics
		{
			get; private set;
		}
		public string Datum
		{
			get; private set;
		}
		public string Title
		{
			get; private set;
		}
		readonly CoreApiDbContext context;
	}
}