using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.InternalApi.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Platform
{
    public abstract class JsonRpcNotificationHandler<TNotification> : IJsonRpcNotificationHandler<TNotification> 
        where TNotification : IRequest, IRequest<Unit>
    {
        protected JsonRpcNotificationHandler(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected abstract Task HandleAsync(TNotification notification);

        public async Task Handle(TNotification request, CancellationToken cancellationToken) => 
            await ((IRequestHandler<TNotification, Unit>)this).Handle(request, cancellationToken);

        async Task<Unit> IRequestHandler<TNotification, Unit>.Handle(TNotification request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogTrace($"Handling notification: {GetType().Name}");

                var handler = Task.Run(async () => await HandleAsync(request));
                var elapsed = await TimedAction.RunAsync(handler);

                Logger.LogTrace($"[PERF] {GetType().Name} completed in {elapsed.TotalMilliseconds:N0} ms.");
                return Unit.Value;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.ToString());
                throw;
            }
        }
    }
}
