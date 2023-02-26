using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class ParametersView : View<ParameterInfo>
    {
        public ParametersView(IDbConnection connection) : base(connection) { }

        protected override string Source { get; } = "[Parameters_v1]";

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "Position",
            "IsParamArray",
            "IsOptional",
            "IsByRef",
            "IsByVal",
            "IsModifierImplicit",
            "DefaultValue",

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

        public async Task<IEnumerable<ParameterInfo>> GetByMemberDeclarationIdAsync(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [MemberDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }

        public async Task<IEnumerable<ParameterInfo>> GetByModuleDeclarationIdAsync(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }

        public async Task<IEnumerable<ParameterInfo>> GetByProjectDeclarationIdAsync(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }

        public override Task<IEnumerable<ParameterInfo>> GetByOptionsAsync<TOptions>(TOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
