using System;
using System.Threading.Tasks;

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ShareInvest
{
	public class CustomAuthorizationDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, IPersistedGrantDbContext where TUser : IdentityUser<TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
	{
		public CustomAuthorizationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> store) : base(options) => this.store = store;
		public DbSet<PersistedGrant> PersistedGrants
		{
			get; set;
		}
		public DbSet<DeviceFlowCodes> DeviceFlowCodes
		{
			get; set;
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(store.Value);
		}
		Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();
		readonly IOptions<OperationalStoreOptions> store;
	}
}