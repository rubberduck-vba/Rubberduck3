using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.Services;

namespace Rubberduck.DatabaseServer.Internal.HealthChecks
{
    internal class HealthCheckService : IHealthCheckService
    {
        private readonly IUnitOfWorkFactory _factory;
        private readonly IEnumerable<HealthCheck> _healthChecks;

        public HealthCheckService(IUnitOfWorkFactory factory, IEnumerable<HealthCheck> healthChecks)
        {
            _factory = factory;
            _healthChecks = healthChecks;
        }

        public async Task<IEnumerable<HealthCheckReport>> RunHealthChecksAsync()
        {
            try
            {
                using (var uow = _factory.CreateNew())
                {
                    var tasks = _healthChecks.Select(check => Task.Run(() => check.RunAsync(uow)));
                    return await Task.WhenAll(tasks);
                }
            }
            catch (Exception exception)
            {
                return new[] { HealthCheckReport.Failure(GetType().Name, "Healthcheck threw an excpetion.", exception) };
            }
        }
    }
}
