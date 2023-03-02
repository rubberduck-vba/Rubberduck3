using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveIdentifierReferenceNotification : SaveNotification<IdentifierReference>
    {
        public SaveIdentifierReferenceNotification(ILogger logger, IUnitOfWorkFactory factory) : base(logger, factory)
        {
        }
    }
}
