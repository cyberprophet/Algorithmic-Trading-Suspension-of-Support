using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using ShareInvest.Models;

namespace ShareInvest
{
	public class CoreApiDbContext : ApiAuthorizationDbContext<CoreUser>
	{
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(store.Value);
			builder.Entity<Days>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<Futures>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<Models.Options>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<Stocks>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<RevisedStockPrice>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<StocksStrategics>().HasKey(o => new { o.Code, o.Strategics });
			builder.Entity<Consensus>().HasKey(o => new { o.Code, o.Date, o.Quarter });
			builder.Entity<EstimatedPrice>().HasKey(o => new { o.Code, o.Strategics });
			builder.Entity<FinancialStatement>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<QuarterlyFinancialStatements>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<Tick>().HasKey(o => new { o.Code, o.Date });
			builder.Entity<Security>().HasKey(o => new { o.Identify, o.Code });
			builder.Entity<Connection>().HasKey(o => new { o.Email, o.Kiwoom });
		}
		public CoreApiDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> store) : base(options, store) => this.store = store;
		public DbSet<Connection> User
		{
			get; set;
		}
		public DbSet<Privacy> Privacies
		{
			get; set;
		}
		public DbSet<SatisfyConditions> Conditions
		{
			get; set;
		}
		public DbSet<Codes> Codes
		{
			get; set;
		}
		public DbSet<Days> Days
		{
			get; set;
		}
		public DbSet<Stocks> Stocks
		{
			get; set;
		}
		public DbSet<Futures> Futures
		{
			get; set;
		}
		public DbSet<Models.Options> Options
		{
			get; set;
		}
		public DbSet<CatalogStrategics> Catalog
		{
			get; set;
		}
		public DbSet<RevisedStockPrice> RevisedStockPrices
		{
			get; set;
		}
		public DbSet<StocksStrategics> StocksStrategics
		{
			get; set;
		}
		public DbSet<Consensus> Consensus
		{
			get; set;
		}
		public DbSet<EstimatedPrice> Estimate
		{
			get; set;
		}
		public DbSet<FileOfGoblinBat> File
		{
			get; set;
		}
		public DbSet<FinancialStatement> Financials
		{
			get; set;
		}
		public DbSet<QuarterlyFinancialStatements> Quarter
		{
			get; set;
		}
		public DbSet<IncorporatedStocks> Incorporate
		{
			get; set;
		}
		public DbSet<Security> Securities
		{
			get; set;
		}
		public DbSet<Tick> Ticks
		{
			get; set;
		}
		readonly IOptions<OperationalStoreOptions> store;
	}
}