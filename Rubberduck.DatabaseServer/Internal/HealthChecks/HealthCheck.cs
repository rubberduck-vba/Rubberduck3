using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model;

namespace Rubberduck.DatabaseServer.Internal.HealthChecks
{
    internal abstract class HealthCheck
    {
        protected abstract Task<HealthCheckReport> GetReportAsync(IUnitOfWork context);

        protected virtual string Name => GetType().Name;
        protected virtual string SuccessMessage { get; } = "Check successful";
        protected virtual string FailureMessage { get; } = "Check failed";

        public async Task<HealthCheckReport> RunAsync(IUnitOfWork context)
        {
            try
            {
                return await GetReportAsync(context);
            }
            catch (Exception exception)
            {
                return HealthCheckReport.Failure(Name, FailureMessage, exception);
            }
        }
    }
}
