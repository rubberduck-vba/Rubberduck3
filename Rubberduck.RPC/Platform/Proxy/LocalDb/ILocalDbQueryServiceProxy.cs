using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbQueryServiceProxy
    {
        /// <summary>
        /// Gets all the <c>ProjectInfo</c> items that match the specified options.
        /// </summary>
        /// <param name="options">The request options representing the criteria to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryProjects, Direction.Bidirectional)]
        Task<IEnumerable<ProjectInfo>> GetProjectsAsync(ProjectInfoRequestOptions options);
        /// <summary>
        /// Gets a <c>ProjectInfo</c> item by its primary key.
        /// </summary>
        /// <param name="options">The request options representing the primary key value to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryProjectInfo, Direction.Bidirectional)]
        Task<ProjectInfo> GetProjectAsync(PrimaryKeyRequestOptions options);

        /// <summary>
        /// Gets all the <c>ModuleInfo</c> items that match the specified options.
        /// </summary>
        /// <param name="options">The request options representing the criteria to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryModules, Direction.Bidirectional)]
        Task<IEnumerable<ModuleInfo>> GetModulesAsync(ModuleInfoRequestOptions options);
        /// <summary>
        /// Gets a <c>ModuleInfo</c> item by its primary key.
        /// </summary>
        /// <param name="options">The request options representing the primary key value to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryModuleInfo, Direction.Bidirectional)]
        Task<ModuleInfo> GetModuleAsync(PrimaryKeyRequestOptions options);

        /// <summary>
        /// Gets all the <c>MemberInfo</c> items that match the specified options.
        /// </summary>
        /// <param name="options">The request options representing the criteria to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryMembers, Direction.Bidirectional)]
        Task<IEnumerable<MemberInfo>> GetMembersAsync(MemberInfoRequestOptions options);
        /// <summary>
        /// Gets a <c>MemberInfo</c> item by its primary key.
        /// </summary>
        /// <param name="options">The request options representing the primary key value to filter for.</param>
        [Method(JsonRpcMethods.Database.QueryMemberInfo, Direction.Bidirectional)]
        Task<MemberInfo> GetMemberAsync(PrimaryKeyRequestOptions options);
    }
}
