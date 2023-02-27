using System.Data;
using System.Threading.Tasks;
using Dapper;
using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;

namespace Rubberduck.Server.LocalDb.Internal.Storage
{
    internal class IdentifierReferenceAnnotationRepository : Repository<IdentifierReferenceAnnotation>
    {
        public IdentifierReferenceAnnotationRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[IdentifierReferenceAnnotations]";

        protected override string[] ColumnNames { get; } = new[]
        {
            "IdentifierReferenceId",
            "AnnotationName",
            "AnnotationArgs",
            "ContextStartOffset",
            "ContextEndOffset",
            "IdentifierStartOffset",
            "IdentifierEndOffset",
        };

        public override async Task SaveAsync(IdentifierReferenceAnnotation entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, 
                    new 
                    {
                        identifierReferenceId = entity.IdentifierReferenceId,
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
                        identifierReferenceId = entity.IdentifierReferenceId,
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
