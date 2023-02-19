using OmniSharp.Extensions.JsonRpc;
using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System;
using System.Linq;
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
            try
            {
                using (var uow = _factory.CreateNew())
                {
                    var repo = uow.GetRepository<T>();
                    var entities = request.Params.ToObject<T[]>();
                    await Task.WhenAll(entities.Select(repo.SaveAsync));
                }

                return new SaveResult { Success = true };
            }
            catch (Exception exception)
            {
                return new SaveResult { Success = false, Message = exception.ToString() };
            }
        }
    }
}
