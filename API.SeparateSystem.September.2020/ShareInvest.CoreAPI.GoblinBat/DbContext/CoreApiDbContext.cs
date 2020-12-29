using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.CoreAPI
{
	public class CoreApiDbContext : DbContext
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Days>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<Futures>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<Options>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<Stocks>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<RevisedStockPrice>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<StocksStrategics>().HasKey(o => new { o.Code, o.Strategics });
			modelBuilder.Entity<Consensus>().HasKey(o => new { o.Code, o.Date, o.Quarter });
			modelBuilder.Entity<EstimatedPrice>().HasKey(o => new { o.Code, o.Strategics });
			modelBuilder.Entity<FinancialStatement>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<QuarterlyFinancialStatements>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<Tick>().HasKey(o => new { o.Code, o.Date });
			modelBuilder.Entity<Models.Security>().HasKey(o => new { o.Identify, o.Code });
		}
		public CoreApiDbContext(DbContextOptions<CoreApiDbContext> options) : base(options)
		{

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
		public DbSet<Options> Options
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
		public DbSet<Models.Security> Securities
		{
			get; set;
		}
		public DbSet<Tick> Ticks
		{
			get; set;
		}
	}
}