using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class ModulesView : View<ModuleInfo>
    {
        public ModulesView(IDbConnection connection)
            : base(connection) { }

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "Folder",
            "DeclarationId",
            "DeclarationType",
            "IdentifierName",
            "DocString",
            "IsUserDefined",
            "ProjectDeclarationId",
            "ProjectName",
            "VBProjectId",
        };

        protected override string Source { get; } = "[Modules_v1]";

        public override Task<IEnumerable<ModuleInfo>> GetByOptionsAsync<TOptions>(TOptions options)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<ModuleInfo>> GetByProjectDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<ModuleInfo>(sql, id);
        }
    }
}
