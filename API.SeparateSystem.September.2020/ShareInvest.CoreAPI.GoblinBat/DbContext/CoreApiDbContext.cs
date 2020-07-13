using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ShareInvest.Models;

namespace ShareInvest.CoreAPI
{
    public class CoreApiDbContext : DbContext
    {
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