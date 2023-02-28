using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbWriterServiceProxy
    {
        [Method(JsonRpcMethods.Database.SaveProject, Direction.ClientToServer)]
        Task SaveProjectGraphAsync(Project graph);
        
        [Method(JsonRpcMethods.Database.SaveModule, Direction.ClientToServer)]
        Task SaveModuleGraphAsync(Module graph);
        
        [Method(JsonRpcMethods.Database.SaveMember, Direction.ClientToServer)]
        Task SaveMemberGraphAsync(Member graph);
    }
}
