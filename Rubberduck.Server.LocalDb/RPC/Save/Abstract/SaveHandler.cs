using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveHandler<TEntity> : JsonRpcRequestHandler<SaveRequest<TEntity>, SaveResult>
        where TEntity : DbEntity, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        protected SaveHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger)
        {
            _factory = factory;
        }

        protected async override Task<SaveResult> HandleAsync(SaveRequest<TEntity> request)
        {
            using (var uow = _factory.CreateNew())
            {
                var repo = uow.GetRepository<TEntity>();
                var entities = request.Params.ToObject<TEntity[]>();

                await Task.WhenAll(entities.Select(repo.SaveAsync));
            }

            return new SaveResult { Success = true };
        }
    }
}
