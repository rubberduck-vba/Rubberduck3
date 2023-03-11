using OmniSharp.Extensions.JsonRpc;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.Services;
using Rubberduck.ServerPlatform.Model;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;

namespace Rubberduck.DatabaseServer
{
    internal class Application : IServerStateService
    {
        private readonly ILogger _logger;
        private readonly IJsonRpcServer _server;
        private readonly IHealthCheckService _healthCheckService;

        public Application(ILogger<Application> logger, IJsonRpcServer server, IHealthCheckService healthCheckService)
        {
            _logger = logger;
            _server = server;
            _healthCheckService = healthCheckService;
        }

        public bool AddClient(ClientProcessInfo client)
        {
            throw new NotImplementedException();
        }

        public ServerProcessInfo GetServerProcessInfo()
        {
            throw new NotImplementedException();
        }

        public bool RemoveClient(ClientProcessInfo client)
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync()
        {
            _server.SendNotification(JsonRpcMethods.DatabaseServer.HeartBeat);
            await StartDatabaseAsync();
        }

        private async Task StartDatabaseAsync()
        {
            var (elapsed, reports) = await TimedAction.RunAsync(() => _healthCheckService.RunHealthChecksAsync());

            _logger.LogInformation("Healthchecks completed in {elapsed} ms", elapsed.TotalMilliseconds);
            foreach (var report in reports)
            {
                if (report.IsSuccess)
                {
                    _logger.LogInformation($"{report.HealthCheck} {report.Message} ({report.Items.Count()} items)");
                    foreach (var item in report.Items)
                    {
                        _logger.LogInformation("\t•{Name} | {ValueDescription}: {Value}", item.Name, item.ValueDescription, item.Value);
                    }
                }
                else
                {
                    _logger.LogError("\t•{Message}\n{Exception}", report.Message, report.Exception);
                }
            }

        }
    }
}