using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.RPC.Save
{
    public class SaveLocalNotification : SaveNotification<Local>
    {
        public SaveLocalNotification(ILogger logger, IUnitOfWorkFactory factory) : base(logger, factory)
        {
        }
    }
}
