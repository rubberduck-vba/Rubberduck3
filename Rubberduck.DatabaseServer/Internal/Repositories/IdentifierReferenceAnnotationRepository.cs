using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.Storage
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
