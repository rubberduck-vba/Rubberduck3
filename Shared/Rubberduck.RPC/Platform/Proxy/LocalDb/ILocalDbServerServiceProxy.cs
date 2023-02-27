using Rubberduck.RPC.Platform.Model.LocalDb.Parameters;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbServerServiceProxy
    {
        Task<SuccessResult> ConnectAsync(ConnectParams parameters);
        Task<SuccessResult> DisconnectAsync(DisconnectParams parameters);

        Task<InfoResult> GetServerInfoAsync();
    }
}
