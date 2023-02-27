using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.RPC.Platform.Model.LocalDb.Responses;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbWriterServiceProxy
    {
        Task<SuccessResult> SaveProjectGraphAsync(Project graph);
        Task<SuccessResult> SaveModuleGraphAsync(Module graph);
        Task<SuccessResult> SaveMemberGraphAsync(Member graph);
    }
}
