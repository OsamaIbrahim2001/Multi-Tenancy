using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MultiTenancy.Data
{
    public class AppDataContext : DbContext
    {
        public string TenantId { get; set; }
        private ITenantService _tenantService;
        public AppDataContext(DbContextOptions options, ITenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
            TenantId = _tenantService.GetCurrentTenant()?.TId;
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _tenantService.GetConnectionString();
            if (!connectionString.IsNullOrEmpty())
            {
                var dbProvider = _tenantService.GetDatabaseProvider();
                if (dbProvider?.ToLower() == "mssql")
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
            }
            base.OnConfiguring(optionsBuilder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().Where(e => e.State == EntityState.Added))
            {
                entry.Entity.TenantId = TenantId;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(e => e.TenantId == TenantId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
