using MediatR;
using OmniSharp.Extensions.JsonRpc;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveHandler<T> : IJsonRpcRequestHandler<SaveRequest<T>, SaveResult>
        where T : DbEntity, new()
    {
        private readonly IUnitOfWorkFactory _factory;

        protected SaveHandler(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public async Task<SaveResult> Handle(SaveRequest<T> request, CancellationToken cancellationToken)
        {
            using (var uow = _factory.CreateNew())
            {
                var repo = uow.GetRepository<T>();
                var entities = request.Params.ToObject<T[]>();
                foreach (var entity in entities)
                {
                    await repo.SaveAsync(entity);
                }
            }
        }
    }
}
