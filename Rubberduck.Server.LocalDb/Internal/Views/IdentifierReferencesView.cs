using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using Rubberduck.RPC.Platform.Model.LocalDb;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class IdentifierReferencesView : View<IdentifierReferenceInfo>
    {
        public IdentifierReferencesView(IDbConnection connection) : base(connection) { }

        protected override string Source { get; } = "[IdentifierReferences_v1]";

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "TypeHint",
            
            "IsAssignmentTarget",
            "IsExplicitCallStatement",
            "IsExplicitLetAssignment",
            "IsSetAssignment",
            "IsReDim",
            "IsArrayAccess",
            "IsProcedureCoercion",
            "IsIndexedDefaultMemberAccess",
            "IsNonIndexedDefaultMemberAccess",
            "IsInnerRecursiveDefaultMemberAccess",
            "DefaultMemberRecursionDepth",

            "ReferenceDeclarationId",
            "ReferenceDeclarationType",
            "ReferenceIdentifierName",
            "ReferenceIsUserDefined",
            "ReferenceDocString",

            "QualifyingReferenceId",

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

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByMemberDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [MemberDeclarationId] = @id";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, id);
        }

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByModuleDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @id";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, id);
        }

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByProjectDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, id);
        }

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ReferenceDeclarationId] = @id";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, id);
        }

        public async Task<IEnumerable<IdentifierReferenceInfo>> GetByOffset(int moduleDeclarationId, int offset)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @moduleDeclarationId AND [ContextStartOffset] <= @offset AND [ContextEndOffset] >= @offset";
            return await Database.QueryAsync<IdentifierReferenceInfo>(sql, new { moduleDeclarationId, offset });
        }
    }
}
