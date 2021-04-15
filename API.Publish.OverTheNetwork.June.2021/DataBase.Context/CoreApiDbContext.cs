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
			builder.Entity<Days>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<Futures>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<Models.Options>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<Stocks>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<RevisedStockPrice>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<StocksStrategics>(o => o.HasKey(o => new { o.Code, o.Strategics }));
			builder.Entity<Consensus>(o => o.HasKey(o => new { o.Code, o.Date, o.Quarter }));
			builder.Entity<EstimatedPrice>(o => o.HasKey(o => new { o.Code, o.Strategics }));
			builder.Entity<FinancialStatement>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<QuarterlyFinancialStatements>(o => o.HasKey(o => new { o.Code, o.Date }));
			builder.Entity<Identify>(o => o.HasKey(o => new { o.Security, o.Code }));
			builder.Entity<Connection>(o => o.HasKey(o => new { o.Email, o.Kiwoom }));
			builder.Entity<IncorporatedStocks>(o => o.HasKey(o => o.Code));
			builder.Entity<Rotation>(o => o.HasKey(o => new { o.Date, o.Code }));
			builder.Entity<StockTags>(o =>
			{
				o.ToTable(nameof(Codes));
				o.HasKey(o => o.Code);
			});
			builder.Entity<Codes>(o =>
			{
				o.ToTable(nameof(Codes));
				o.HasKey(o => o.Code);
				o.HasOne(o => o.Tags).WithOne().HasForeignKey<StockTags>(o => o.Code);
				o.HasMany(o => o.Rotations).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Days).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Stocks).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Futures).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Options).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Consensus).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Estimate).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Financials).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.RevisedStockPrices).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.StocksStrategics).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Quarter).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Securities).WithOne().HasForeignKey(o => o.Code);
				o.HasMany(o => o.Incorporate).WithOne().HasForeignKey(o => o.Code);
			});
			builder.Entity<Tick>(o =>
			{
				o.ToTable(tick);
				o.HasOne(o => o.Contents).WithOne().HasForeignKey<Contents>(o => new { o.Code, o.Date });
				o.HasKey(o => new { o.Code, o.Date });
			});
			builder.Entity<Contents>(o =>
			{
				o.ToTable(tick);
				o.HasKey(o => new { o.Code, o.Date });
			});
			builder.Entity<Tendency>(o => o.HasKey(o => new { o.Code, o.Tick }));
			builder.Entity<Response>(o =>
			{
				o.ToTable(typeof(Group).Name);
				o.HasKey(o => o.Code);
			});
			builder.Entity<GroupDetail>(o =>
			{
				o.ToTable(typeof(Group).Name);
				o.HasMany(o => o.Tendencies).WithOne().HasForeignKey(o => o.Code);
				o.HasKey(o => o.Code);
			});
			builder.Entity<Group>(o =>
			{
				o.ToTable(typeof(Group).Name);
				o.HasKey(o => o.Code);
				o.HasOne(o => o.Details).WithOne().HasForeignKey<GroupDetail>(o => o.Code);
				o.HasOne(o => o.Page).WithOne().HasForeignKey<Response>(o => o.Code);
			});
			builder.Entity<Url>(o =>
			{
				o.ToTable(nameof(Theme));
				o.HasKey(o => o.Index);
			});
			builder.Entity<ThemeTags>(o =>
			{
				o.ToTable(nameof(Theme));
				o.HasKey(o => o.Index);
			});
			builder.Entity<Theme>(o =>
			{
				o.ToTable(nameof(Theme));
				o.HasKey(o => o.Index);
				o.HasOne(o => o.Url).WithOne().HasForeignKey<Url>(o => o.Index);
				o.HasOne(o => o.Tags).WithOne().HasForeignKey<ThemeTags>(o => o.Index);
				o.HasMany(o => o.Groups).WithOne().HasForeignKey(o => o.Index);
			});
		}
		public CoreApiDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> store) : base(options, store) => this.store = store;
		public DbSet<Rotation> Rotations
		{
			get; set;
		}
		public DbSet<Connection> User
		{
			get; set;
		}
		public DbSet<Tendency> Tendencies
		{
			get; set;
		}
		public DbSet<GroupDetail> Details
		{
			get; set;
		}
		public DbSet<Response> Page
		{
			get; set;
		}
		public DbSet<Group> Group
		{
			get; set;
		}
		public DbSet<Url> Url
		{
			get; set;
		}
		public DbSet<ThemeTags> ThemeTags
		{
			get; set;
		}
		public DbSet<Theme> Theme
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
		public DbSet<StockTags> StockTags
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
		public DbSet<Identify> Securities
		{
			get; set;
		}
		public DbSet<Tick> Ticks
		{
			get; set;
		}
		public DbSet<Contents> Contents
		{
			get; set;
		}
		readonly IOptions<OperationalStoreOptions> store;
		const string tick = "Ticks";
	}
}