using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class LocalsView : View<LocalInfo>
    {
        public LocalsView(IDbConnection connection) : base(connection) { }

        protected override string Source { get; } = "[Locals_v1]";

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "IsAutoAssigned",
            "ValueExpression",

            "DeclarationId",
            "DeclarationType",
            "IdentifierName",
            "DocString",
            "IsUserDefined",
            "AsTypeName",
            "AsTypeDeclarationId",
            "IsArray",
            "IsImplicit",
            "TypeHint",

            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",

            "MemberDeclarationId",
            "MemberDeclarationType",
            "MemberName",

            "ModuleDeclarationId",
            "ModuleDeclarationType",
            "ModuleName",
            "Folder",

            "ProjectDeclarationId",
            "ProjectName",
            "VBProjectId",
        };

        public async Task<IEnumerable<LocalInfo>> GetByMemberDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [MemberDeclarationId] = @id";
            return await Database.QueryAsync<LocalInfo>(sql, id);
        }

        public async Task<IEnumerable<LocalInfo>> GetByModuleDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @id";
            return await Database.QueryAsync<LocalInfo>(sql, id);
        }

        public async Task<IEnumerable<LocalInfo>> GetByProjectDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<LocalInfo>(sql, id);
        }
    }
}
