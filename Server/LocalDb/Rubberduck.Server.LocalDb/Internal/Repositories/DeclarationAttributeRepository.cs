using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
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
