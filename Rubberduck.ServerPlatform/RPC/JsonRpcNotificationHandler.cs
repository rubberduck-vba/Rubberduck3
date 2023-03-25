﻿using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.InternalApi.Common;

namespace Rubberduck.ServerPlatform.RPC
{
    public abstract class JsonRpcNotificationHandler<TParameter> : IJsonRpcNotificationHandler<TParameter> 
        where TParameter : IRequest
    {
        protected JsonRpcNotificationHandler(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected abstract Task HandleAsync(TParameter parameter);

        async Task<Unit> IRequestHandler<TParameter, Unit>.Handle(TParameter request, CancellationToken cancellationToken)
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