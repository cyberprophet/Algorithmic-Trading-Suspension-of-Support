using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
        }
        public CoreApiDbContext(DbContextOptions<CoreApiDbContext> options) : base(options) => IsDebugging(options.ContextType);
        public DbSet<Privacy> Privacies
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
        static ulong Count
        {
            get; set;
        }
        [Conditional("DEBUG")]
        void IsDebugging(Type context) => new Task(() =>
        {
            Console.WriteLine(Count++ + "\t" + DateTime.Now + "\t" + context.Name);
            var now = DateTime.Now;

            if ((now.Hour == 9 && now.Minute == 0 && now.Second == 0) || Count == ulong.MaxValue)
                Count = ulong.MinValue;
        }).Start();
    }
}