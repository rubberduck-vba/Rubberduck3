using Microsoft.Extensions.Logging;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveParameterNotification : SaveNotification<Parameter>
    {
        public SaveParameterNotification(ILogger logger, IUnitOfWorkFactory factory) : base(logger, factory)
        {
        }
    }
}
