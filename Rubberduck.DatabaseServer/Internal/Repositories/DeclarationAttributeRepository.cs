using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.RPC.Platform.Model.Database;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class DeclarationAttributeRepository : Repository<DeclarationAttribute>
    {
        public DeclarationAttributeRepository(IDbConnection connection)
            : base(connection) { }

        protected override string Source { get; } = "[DeclarationAttributes]";
        protected override string[] ColumnNames { get; } = new[]
        {
            "DeclarationId",
            "AttributeName",
            "AttributeValues",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
        };

        public override async Task SaveAsync(DeclarationAttribute entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        attributeName = entity.AttributeName,
                        attributeValues = entity.AttributeValues,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.IdentifierStartOffset,
                        identifierEndOffset = entity.IdentifierEndOffset
                    });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql,
                    new
                    {
                        declarationId = entity.DeclarationId,
                        attributeName = entity.AttributeName,
                        attributeValues = entity.AttributeValues,
                        contextStartOffset = entity.ContextStartOffset,
                        contextEndOffset = entity.ContextEndOffset,
                        identifierStartOffset = entity.IdentifierStartOffset,
                        identifierEndOffset = entity.IdentifierEndOffset,
                        id = entity.Id
                    });
            }
        }
    }
}
