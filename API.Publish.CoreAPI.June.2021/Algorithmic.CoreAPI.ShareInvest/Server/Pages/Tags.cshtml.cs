using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ShareInvest.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
	public class TagsModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync()
		{
			Title = Request.Query[name];
			Index = Request.Query[index];
			var sb = new StringBuilder();

			foreach (var str in Request.Query[kiwoom].ToString().Split('/'))
				if (int.TryParse(str, out int confirm))
					sb.Append((char)confirm);

			if (await context.User.AnyAsync(o => o.Email.Equals(sb.ToString())))
			{
				switch (Index.Length)
				{
					case 6:
						if (context.StockTags.Find(Index) is Models.StockTags st)
						{
							ID = st.ID;
							Json = st.Tags;
							Size = st.Size;
						}
						else
							return NotFound();

						break;

					default:
						if (context.ThemeTags.Find(Index) is Models.ThemeTags ts)
						{
							ID = ts.ID;
							Json = ts.Tags;
							Size = ts.Size;
						}
						else
							return NotFound();

						break;
				}
				return Page();
			}
			return Unauthorized();
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
		const string kiwoom = "im";
	}
}