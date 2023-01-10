using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.DataServer.Abstract;
using Rubberduck.DataServer.Entities;

namespace Rubberduck.DataServer.Views
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
            "MemberName",
            
            "ModuleDeclarationId",
            "ModuleName",
            "Folder",

            "ProjectDeclarationId",
            "ProjectName",
            "VBProjectId",
        };

        public async Task<IEnumerable<ParameterInfo>> GetByMemberDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [MemberDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }

        public async Task<IEnumerable<ParameterInfo>> GetByModuleDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ModuleDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }

        public async Task<IEnumerable<ParameterInfo>> GetByProjectDeclarationId(int id)
        {
            var sql = $"SELECT {Columns} FROM {Source} WHERE [ProjectDeclarationId] = @id";
            return await Database.QueryAsync<ParameterInfo>(sql, id);
        }
    }
}
