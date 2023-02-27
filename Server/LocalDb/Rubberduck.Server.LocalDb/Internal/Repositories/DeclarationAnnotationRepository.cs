using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class DeclarationAnnotationRepository : Repository<DeclarationAnnotation>
    {
        public DeclarationAnnotationRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[DeclarationAnnotations]";
        protected override string[] ColumnNames { get; } = new[]
        {
            "DeclarationId",
            "AnnotationName",
            "AnnotationArgs",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
        };

        public override async Task SaveAsync(DeclarationAnnotation entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql,
                    new 
                    { 
                        declarationId = entity.DeclarationId,
                        annotationName = entity.AnnotationName,
                        annotationArgs = entity.AnnotationArgs,
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
                        annotationName = entity.AnnotationName,
                        annotationArgs = entity.AnnotationArgs,
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
