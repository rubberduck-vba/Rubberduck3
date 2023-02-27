using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Query
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

        protected override Task<QueryResult<TResult>> HandleAsync(QueryRequest<TResult, TOptions> request) => throw new NotSupportedException();

        protected abstract Task<QueryResult<TResult>> HandleAsync(QueryRequest<TResult, TOptions> request, IUnitOfWork uow);

        public sealed async override Task<QueryResult<TResult>> Handle(QueryRequest<TResult, TOptions> request, CancellationToken cancellationToken)
        {
            using (var uow = _factory.CreateNew())
            {
                try
                {
                    Logger.LogTrace($"Handling notification: {GetType().Name}");
                    QueryResult<TResult> response = default;

                    var handler = Task.Run(async () => response = await HandleAsync(request, uow));
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
