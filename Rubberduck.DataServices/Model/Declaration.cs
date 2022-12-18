using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public class Declaration
    {
        public bool IsMarkedForDeletion { get; set; }
        public bool IsMarkedForUpdate { get; set; }

        public int Id { get; set; }
        public LocationInfo LocationInfo { get; set; }
        public InternalApi.Model.DeclarationType DeclarationType { get; set; }
        public bool IsArray { get; set; }
        public string AsTypeName { get; set; }
        public Declaration AsTypeDeclaration { get; set; }
        public string TypeHint { get; set; }
        public string IdentifierName { get; set; }
        public bool IsUserDefined { get; set; }
        public Declaration ParentDeclaration { get; set; }
        public string DocString { get; set; }
        public string[] Annotations { get; set; }
        public string[] Attributes { get; set; }

        public IEnumerable<IdentifierReference> IdentifierReferences { get; set; } = new IdentifierReference[] { };

        internal Entities.Declaration ToEntity()
        {
            return new Entities.Declaration
            {
                Id = Id,
                IsMarkedForUpdate = IsMarkedForUpdate ? 1 : 0,
                IsMarkedForDeletion = IsMarkedForDeletion ? 1: 0,

                Annotations = string.Join("|", Annotations),
                AsTypeDeclarationId = AsTypeDeclaration?.Id,
                AsTypeName = AsTypeName,
                Attributes = string.Join("|", Attributes),
                ContextStartOffset = LocationInfo.ContextOffset.Start,
                ContextEndOffset = LocationInfo.ContextOffset.End,
                DeclarationType = (long)DeclarationType,
                DocString = DocString,
                DocumentLineEnd = LocationInfo.DocumentLineEnd,
                DocumentLineStart = LocationInfo.DocumentLineStart,
                HighlightStartOffset = LocationInfo.HighlightOffset.Start,
                HighlightEndOffset = LocationInfo.HighlightOffset.End,
                IdentifierName = IdentifierName,
                IsArray = IsArray ? 1 : 0,
                IsUserDefined = IsUserDefined ? 1 : 0,
                ParentDeclarationId = ParentDeclaration?.Id,
                TypeHint = TypeHint,
            };
        }

        internal static async Task<Declaration> FromEntityAsync(Entities.Declaration entity, Repository<Entities.Declaration> repository)
        {
            var result = new Declaration
            {
                Id = entity.Id,
                IsMarkedForUpdate = entity.IsMarkedForUpdate != 0,
                IsMarkedForDeletion = entity.IsMarkedForDeletion != 0,

                AsTypeName = entity.AsTypeName,
                DeclarationType = (DeclarationType)entity.DeclarationType,
                DocString = entity.DocString,
                IdentifierName = entity.IdentifierName,
                IsArray = entity.IsArray != 0,
                IsUserDefined = entity.IsUserDefined != 0,
                TypeHint = entity.TypeHint,

                Annotations = entity.Annotations.Split('|'),
                Attributes = entity.Attributes.Split('|'),
            };

            result.LocationInfo = result.IsUserDefined
                ? new LocationInfo
                {
                    DocumentLineStart = entity.DocumentLineStart ?? 0,
                    DocumentLineEnd = entity.DocumentLineEnd ?? 0,
                    ContextOffset = new DocumentOffset(entity.ContextStartOffset ?? 0, entity.ContextEndOffset ?? -1),
                    HighlightOffset = new DocumentOffset(entity.HighlightStartOffset ?? 0, entity.HighlightEndOffset ?? -1)
                }
                : null;

            if (entity.ParentDeclarationId.HasValue)
            {
                var parent = await repository.GetByIdAsync(entity.ParentDeclarationId.Value);
                result.ParentDeclaration = await FromEntityAsync(parent, repository);
            }

            if (entity.AsTypeDeclarationId.HasValue)
            {
                var asType = await repository.GetByIdAsync(entity.AsTypeDeclarationId.Value);
                result.AsTypeDeclaration = await FromEntityAsync(asType, repository);
            }

            return result;
        }
    }
}
