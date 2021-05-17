using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShareInvest.Pages
{
	public class ConcatModel : PageModel
	{
		public List<Tuple<string, byte[]>> Files
		{
			get; set;
		}
		public IActionResult OnGet()
		{
			Files = new List<Tuple<string, byte[]>>();

			foreach (var file in resources)
				Files.Add(new Tuple<string, byte[]>(file.Key, file.Value));

			return Page();
		}
		public ConcatModel() => resources = new()
		{
			{
				nameof(Properties.Resources._1)[1..],
				Properties.Resources._1
			},
			{
				nameof(Properties.Resources._2)[1..],
				Properties.Resources._2
			},
			{
				nameof(Properties.Resources._3)[1..],
				Properties.Resources._3
			}
		};
		readonly Dictionary<string, byte[]> resources;
	}
}