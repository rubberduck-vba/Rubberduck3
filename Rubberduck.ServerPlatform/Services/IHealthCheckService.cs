using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.ServerPlatform.Services
{
    public interface IHealthCheckService
    {
        Task<IEnumerable<HealthCheckReport>> RunHealthChecksAsync();
    }
}
