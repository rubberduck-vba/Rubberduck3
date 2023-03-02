using MediatR;
using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public abstract class SaveNotification<TEntity> : SaveNotificationHandler<TEntity>, INotification, IRequest<Unit>
        where TEntity : DbEntity, IRequest, new()
    {
        protected SaveNotification(ILogger logger, IUnitOfWorkFactory factory) : base(logger, factory)
        {
        }
    }
}
