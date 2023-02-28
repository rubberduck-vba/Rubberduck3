using Rubberduck.RPC.Platform.Model.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface IDatabaseQueryServiceProxy
    {
        Task<IEnumerable<ProjectInfo>> GetProjectsAsync(PrimaryKeyRequestOptions options);
        Task<ProjectInfo> GetProjectAsync(ProjectInfoRequestOptions options);

        Task<IEnumerable<ModuleInfo>> GetModulesAsync(PrimaryKeyRequestOptions options);
        Task<ModuleInfo> GetModuleAsync(ModuleInfoRequestOptions options);

    }
}
