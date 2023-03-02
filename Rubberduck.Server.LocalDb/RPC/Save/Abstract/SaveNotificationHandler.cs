using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.InternalApi.Common;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    [Method(JsonRpcMethods.Database.SaveMember, Direction.ClientToServer)]
    public abstract class SaveNotificationHandler<TEntity> : JsonRpcNotificationHandler<SaveParams<TEntity>>
        where TEntity : DbEntity, IRequest, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        protected SaveNotificationHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger)
        {
            _factory = factory;
        }

        protected async override Task HandleAsync(SaveParams<TEntity> request)
        {
            var elapsed = await TimedAction.RunAsync(SaveEntitiesAsync(request.Entities));
            Logger.LogTrace($"[PERF] {GetType().Name}.SaveEntitiesAsync completed in {elapsed.TotalMilliseconds:N0} ms.");
        }

        private async Task SaveEntitiesAsync(IEnumerable<TEntity> entities)
        {
            using (var uow = _factory.CreateNew())
            {
                var repo = uow.GetRepository<TEntity>();
                foreach (var entity in entities)
                {
                    await repo.SaveAsync(entity);
                }

                uow.SaveChanges();
            }
        }
    }
}
