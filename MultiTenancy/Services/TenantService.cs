using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace MultiTenancy.Services
{
    public class TenantService : ITenantService
    {
        private readonly TenantSettings _tenantSettings;
        private Tenant? _currentTenant;
        private HttpContext? _httpContext;

        public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSettings> tenantSettings)
        {
            _httpContext = contextAccessor.HttpContext;
            _tenantSettings = tenantSettings.Value;
            if (_httpContext is not null)
            {
                if (_httpContext.Request.Headers.TryGetValue("tenant", out var tenantId))
                {
                    SetConnectionString(tenantId);
                }
                else throw new Exception("No Tenant Provided");
            }
        }

        private void SetConnectionString(StringValues tenantId)
        {
            _currentTenant = _tenantSettings.Tenants.FirstOrDefault(t => t.TId == tenantId);
            if (_currentTenant is null)
                throw new Exception("Invalid Tenant ID");
            if (string.IsNullOrEmpty(_currentTenant.ConnectionString))
                _currentTenant.ConnectionString = _tenantSettings.Default.ConnectionString;
        }

        public string? GetConnectionString()
        {
            var currentConnectionString = _currentTenant is null ? _tenantSettings.Default.ConnectionString : _currentTenant.ConnectionString;
            return currentConnectionString;
        }

        public Tenant? GetCurrentTenant()
        {
            return _currentTenant;
        }

        public string? GetDatabaseProvider()
        {
            return _tenantSettings.Default.DBProvider;
        }
    }
}
