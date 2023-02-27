using Rubberduck.RPC.Platform.Model.LocalDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbQueryServiceProxy
    {
        Task<IEnumerable<ProjectInfo>> GetProjectsAsync(ProjectInfoRequestOptions options);
        Task<ProjectInfo> GetProjectAsync(PrimaryKeyRequestOptions options);

        Task<IEnumerable<ModuleInfo>> GetModulesAsync(ModuleInfoRequestOptions options);
        Task<ModuleInfo> GetModuleAsync(PrimaryKeyRequestOptions options);
    }

    public interface ILocalDbWriterServiceProxy
    {
        Task<ProjectInfo> SaveProjectAsync(ProjectInfo item);
        Task SaveProjectsAsync(IEnumerable<ProjectInfo> items);
    }
}
