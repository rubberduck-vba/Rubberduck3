using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform.Platform;
using Rubberduck.ServerPlatform.Platform.Model.Database;
using Rubberduck.ServerPlatform.Platform.Model.Database.Responses;
using Rubberduck.Server.Database.Internal.Storage.Abstract;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Server.Database.RPC.Save
{
    public abstract class SaveHandler<TEntity> : JsonRpcRequestHandler<SaveRequest<TEntity>, SuccessResult>
        where TEntity : DbEntity, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        protected SaveHandler(ILogger logger, IUnitOfWorkFactory factory)
            : base(logger)
        {
            _factory = factory;
        }

        protected async override Task<SuccessResult> HandleAsync(SaveRequest<TEntity> request)
        {
            using (var uow = _factory.CreateNew())
            {
                var repo = uow.GetRepository<TEntity>();
                var entities = request.Params.ToObject<TEntity[]>();

                await Task.WhenAll(entities.Select(repo.SaveAsync));
            }

            return new SuccessResult();
        }
    }
}
