using Rubberduck.RPC.Platform.Model.Database.Parameters;
using Rubberduck.RPC.Platform.Model.Database.Responses;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface IDatabaseServerServiceProxy
    {
        Task<SuccessResult> ConnectAsync(ConnectParams parameters);
        Task<SuccessResult> DisconnectAsync(DisconnectParams parameters);

        Task<InfoResult> GetServerInfoAsync();
    }
}
