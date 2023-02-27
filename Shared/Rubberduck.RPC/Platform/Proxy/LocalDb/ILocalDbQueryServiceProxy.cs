using Rubberduck.RPC.Platform.Model.LocalDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbQueryServiceProxy
    {
        Task<IEnumerable<ProjectInfo>> GetProjectsAsync(PrimaryKeyRequestOptions options);
        Task<ProjectInfo> GetProjectAsync(ProjectInfoRequestOptions options);

        Task<IEnumerable<ModuleInfo>> GetModulesAsync(PrimaryKeyRequestOptions options);
        Task<ModuleInfo> GetModuleAsync(ModuleInfoRequestOptions options);

    }
}
