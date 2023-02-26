using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class MembersView : View<MemberInfo>
    {
        public MembersView(IDbConnection connection)
            : base(connection) { }

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "ImplementsDeclarationId",
            "Accessibility",
            "IsAutoAssigned",
            "IsWithEvents",
            "IsDimStmt",
            "ValueExpression",
            "DeclarationId",
            "DeclarationType",
            "IdentifierName",
            "DocString",
            "IsUserDefined",
            "AsTypeName",
            "AsTypeDeclarationId",
            "IsArray",
            "TypeHint",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
            "ModuleDeclarationId",
            "ModuleDeclarationType",
            "ModuleName",
            "Folder",
            "ProjectDeclarationId",
            "ProjectName",
            "VBProjectId",
        };

        protected override string Source { get; } = "[Members_v1]";

        public async Task<IEnumerable<MemberInfo>> GetByModuleDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @id";
            return await Database.QueryAsync<MemberInfo>(sql, id);
        }

        public async Task<IEnumerable<MemberInfo>> GetByProjectDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<MemberInfo>(sql, id);
        }
    }
}
