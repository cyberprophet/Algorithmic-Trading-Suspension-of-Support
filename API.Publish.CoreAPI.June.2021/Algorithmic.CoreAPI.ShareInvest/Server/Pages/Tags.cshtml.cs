using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShareInvest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
	public class TagsModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
		{
			Title = Request.Query[name];
			Index = Request.Query[index];
			
			switch (Index.Length)
			{
				case 6:
					if (await context.StockTags.FindAsync(Index) is Models.StockTags st)
					{
						ID = st.ID;
						Json = st.Tags;
						Size = st.Size;
					}
					break;

				default:
					if (await context.ThemeTags.FindAsync(Index) is Models.ThemeTags ts)
					{
						ID = ts.ID;
						Json = ts.Tags;
						Size = ts.Size;
					}
					break;
			}
			return Page();
		}
		public TagsModel(CoreApiDbContext context) => this.context = context;
		public string ID
		{
			get; private set;
		}
		public string Json
		{
			get; private set;
		}
		public string Size
		{
			get; private set;
		}
		public string Index
		{
			get; private set;
		}
		public string Title
		{
			get; private set;
		}
		readonly CoreApiDbContext context;
		const string name = "name";
		const string index = "index";
	}
}