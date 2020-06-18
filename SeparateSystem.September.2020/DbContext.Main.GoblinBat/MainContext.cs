using System.Data.Entity;

using ShareInvest.Models;

namespace ShareInvest.Main
{
    public class MainContext : DbContext
    {
        public DbSet<FileGoblinBat> File
        {
            get; set;
        }
        public MainContext() : base(Secrecy.Port)
        {

        }
    }
}