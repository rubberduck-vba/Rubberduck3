using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.InternalApi.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Platform
{
    public abstract class JsonRpcRequestHandler<TRequest, TResponse> : IJsonRpcRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        protected JsonRpcRequestHandler(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected abstract Task<TResponse> HandleAsync(TRequest request);

        public async virtual Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogTrace($"Handling notification: {GetType().Name}");
                TResponse response = default;

                var handler = Task.Run(async () => response = await HandleAsync(request));
                var elapsed = await TimedAction.RunAsync(handler);

                Logger.LogTrace($"[PERF] {GetType().Name} completed in {elapsed.TotalMilliseconds:N0} ms.");
                return response;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.ToString());
                throw;
            }
        }
    }
}
