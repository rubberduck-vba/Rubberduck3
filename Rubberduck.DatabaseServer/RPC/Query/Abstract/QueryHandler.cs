using Microsoft.Extensions.Logging;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.InternalApi.Common;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.DatabaseServer.RPC.Query.Abstract
{
    public abstract class QueryHandler<TResult, TOptions> : JsonRpcRequestHandler<QueryRequest<TResult, TOptions>, QueryResult<TResult>>
        where TResult : class, new()
        where TOptions : IQueryOption, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        protected QueryHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger)
        {
            _factory = factory;
        }

        protected abstract Task<QueryResult<TResult>> HandleAsync(QueryRequest<TResult, TOptions> request, IUnitOfWork uow, CancellationToken cancellationToken);

        protected sealed override Task<QueryResult<TResult>> HandleAsync(QueryRequest<TResult, TOptions> request)
            => Handle(request, CancellationToken.None);

        public sealed async override Task<QueryResult<TResult>> Handle(QueryRequest<TResult, TOptions> request, CancellationToken cancellationToken)
        {
            using (var uow = _factory.CreateNew())
            {
                try
                {
                    Logger.LogTrace($"Handling request: {GetType().Name}");
                    QueryResult<TResult> response = new QueryResult<TResult>();

                    var handler = Task.Run(async () => response = await HandleAsync(request, uow, cancellationToken));
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
}
