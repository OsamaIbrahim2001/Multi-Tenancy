using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;

namespace MultiTenancy
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddTenancy(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<TenantSettings>(configuration.GetSection(nameof(TenantSettings)));

            TenantSettings options = new();
            configuration.GetSection(nameof(TenantSettings)).Bind(options);

            services.AddScoped<ITenantService, TenantService>();

            var defaultDbProvider = options.Default.DBProvider;
            if (defaultDbProvider.Equals("mssql", StringComparison.CurrentCultureIgnoreCase))
                services.AddDbContext<AppDataContext>(m => m.UseSqlServer());

            foreach (var tenant in options.Tenants)
            {
                var connectionString = tenant.ConnectionString ?? options.Default.ConnectionString;
                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDataContext>();

                dbContext.Database.SetConnectionString(connectionString);

                if (dbContext.Database.GetPendingMigrations().Any())
                    dbContext.Database.Migrate();
            }
            return services;
        }
    }
}
