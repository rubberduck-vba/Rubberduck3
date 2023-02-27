using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class DeclarationRepository : Repository<Declaration>
    {
        public DeclarationRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[Declarations]";
        protected override string[] ColumnNames { get; } = new[]
        { 
            "DeclarationType",
            "IsArray",
            "AsTypeName",
            "AsTypeDeclarationId",
            "TypeHint",
            "IdentifierName",
            "IsUserDefined",
            "ParentDeclarationId",
            "DocString",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
        };

        public async Task<IEnumerable<Declaration>> GetByParentIdAsync(int parentId)
        {
            var sql = $"SELECT [Id],{NonIdentityColumns} FROM {Source} WHERE [ParentDeclarationId] = @parentId;";
            return await Database.QueryAsync<Declaration>(sql, new { parentId });
        }

        public override async Task SaveAsync(Declaration entity)
        {
            if (entity.Id != default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new
                    {
                        declarationType = entity.DeclarationType,
                        isArray = entity.IsArray,
                        asTypeName = entity.AsTypeName,
                        asTypeDeclarationId = entity.AsTypeDeclarationId,
                        typeHint = entity.TypeHint,
                        identifierName = entity.IdentifierName,
                        isUserDefined = entity.IsUserDefined,
                        parentDeclarationId = entity.ParentDeclarationId,
                        docString = entity.DocString,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.IdentifierStartOffset,
                        identifierEndOffset = entity.IdentifierEndOffset,
                    });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql, new
                    {
                        declarationType = entity.DeclarationType,
                        isArray = entity.IsArray,
                        asTypeName = entity.AsTypeName,
                        asTypeDeclarationId = entity.AsTypeDeclarationId,
                        typeHint = entity.TypeHint,
                        identifierName = entity.IdentifierName,
                        isUserDefined = entity.IsUserDefined,
                        parentDeclarationId = entity.ParentDeclarationId,
                        docString = entity.DocString,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.IdentifierStartOffset,
                        identifierEndOffset = entity.IdentifierEndOffset,
                        id = entity.Id,
                    });
            }
        }
    }
}
