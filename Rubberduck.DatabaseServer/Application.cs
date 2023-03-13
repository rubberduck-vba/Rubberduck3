using OmniSharp.Extensions.JsonRpc;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.Services;
using Rubberduck.ServerPlatform.Model;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.ServerPlatform;
using System.Collections.Concurrent;

namespace Rubberduck.DatabaseServer
{
    internal class ServerStateService : IServerStateService
    {
        private readonly ConcurrentDictionary<string, ClientProcessInfo> _clients = new ConcurrentDictionary<string, ClientProcessInfo>();

        public bool AddClient(ClientProcessInfo client)
        {
            return _clients.TryAdd(client.Name, client) || _clients.ContainsKey(client.Name);
        }

        public ServerProcessInfo GetServerProcessInfo()
        {
            throw new NotImplementedException();
        }

        public bool RemoveClient(ClientProcessInfo client)
        {
            return _clients.Remove(client.Name, out _) || !_clients.ContainsKey(client.Name);
        }
    }

    internal class Application : IServerApplication
    {
        private readonly ILogger _logger;
        private readonly IServerStateService _state;
        private readonly IJsonRpcServer _server;
        private readonly IHealthCheckService _healthCheckService;

        public Application(ILogger<Application> logger, IJsonRpcServer server, IHealthCheckService healthCheckService, IServerStateService state)
        {
            _logger = logger;
            _state = state;
            _server = server;
            _healthCheckService = healthCheckService;
        }

        public async Task StartAsync(CancellationToken token)
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