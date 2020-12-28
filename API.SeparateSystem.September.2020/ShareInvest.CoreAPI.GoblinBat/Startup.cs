using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Z.EntityFramework.Extensions;

namespace ShareInvest
{
	public class Startup
	{
		public IConfiguration Configuration
		{
			get;
		}
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<KestrelServerOptions>(o => o.Limits.MaxRequestBodySize = int.MaxValue)
				.AddDbContext<CoreAPI.CoreApiDbContext>(o => o.UseSqlServer(Configuration[Security.Connection]))
				.AddControllersWithViews(o => o.InputFormatters.Insert(0, GetJsonPatchInputformatter()))
				.AddMvcOptions(o => o.EnableEndpointRouting = false)
				.SetCompatibilityVersion(CompatibilityVersion.Latest);
			EntityFrameworkManager.ContextFactory = context
				=> new CoreAPI.CoreApiDbContext(new DbContextOptionsBuilder<CoreAPI.CoreApiDbContext>()
				.UseSqlServer(Configuration[Security.Connection]).Options);
		}
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseMvc();

			else
				app.UseMvc();
		}
		public Startup(IConfiguration configuration) => Configuration = configuration;
		static NewtonsoftJsonInputFormatter GetJsonPatchInputformatter()
			=> new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
			.Services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
	}
}