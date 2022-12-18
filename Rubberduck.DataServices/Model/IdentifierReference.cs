using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public class IdentifierReference
    {
        public bool IsMarkedForDeletion { get; set; }
        public bool IsMarkedForUpdate { get; set; }

        public int Id { get; set; }

        public Declaration ReferencedDeclaration { get; set; }
        public Declaration ParentDeclaration { get; set; }
        public IdentifierReference QualifyingReference { get; set; }
        public LocationInfo LocationInfo { get; set; }
        public bool IsAssignmentTarget { get; set; }
        public bool IsExplicitCallStatement { get; set; }
        public bool IsExplicitLetAssignment { get; set; }
        public bool IsSetAssignment { get; set; }
        public bool IsReDim { get; set; }
        public bool IsArrayAccess { get; set; }
        public bool IsProcedureCoercion { get; set; }
        public bool IsIndexedDefaultMemberAccess { get; set; }
        public bool IsNonIndexedDefaultMemberAccess { get; set; }
        public bool IsInnerRecursiveDefaultMemberAccess { get; set; }
        public int? DefaultMemberRecursionDepth { get; set; }
        public string TypeHint { get; set; }
        public string[] Annotations { get; set; }

        internal Entities.IdentifierReference ToEntity()
        {
            return new Entities.IdentifierReference
            {
                Id = Id,
                IsMarkedForUpdate = IsMarkedForUpdate ? 1 : 0,
                IsMarkedForDeletion = IsMarkedForDeletion ? 1 : 0,

                IsAssignmentTarget = IsAssignmentTarget ? 1 : 0,
                IsExplicitCallStatement = IsExplicitCallStatement ? 1 : 0,
                IsExplicitLetAssignment = IsExplicitLetAssignment ? 1 : 0,
                IsSetAssignment = IsSetAssignment ? 1 : 0,
                IsReDim = IsReDim ? 1 : 0,
                IsArrayAccess = IsArrayAccess ? 1 : 0,
                IsProcedureCoercion = IsProcedureCoercion ? 1 : 0,
                IsIndexedDefaultMemberAccess = IsIndexedDefaultMemberAccess ? 1 : 0,
                IsNonIndexedDefaultMemberAccess = IsNonIndexedDefaultMemberAccess ? 1: 0,
                IsInnerRecursiveDefaultMemberAccess = IsInnerRecursiveDefaultMemberAccess ? 1 : 0,
                DefaultMemberRecursionDepth = DefaultMemberRecursionDepth,
                TypeHint = TypeHint,
                Annotations = string.Join("|", Annotations),
                
                DocumentLineStart = LocationInfo.DocumentLineStart,
                DocumentLineEnd = LocationInfo.DocumentLineEnd,
                ContextStartOffset = LocationInfo.ContextOffset.Start,
                ContextEndOffset = LocationInfo.ContextOffset.End,
                IdentifierStartOffset = LocationInfo.HighlightOffset.Start,
                IdentifierEndOffset = LocationInfo.HighlightOffset.End,
                
                ParentDeclarationId = ParentDeclaration.Id,
                ReferencedDeclarationId = ReferencedDeclaration.Id,
                QualifyingReferenceId = QualifyingReference?.Id,
            };
        }

        internal static async Task<IdentifierReference> FromEntityAsync(Entities.IdentifierReference entity, Repository<Entities.IdentifierReference> entities, Repository<Entities.Declaration> declarations)
        {
            var result = new IdentifierReference
            {
                Id = entity.Id,
                IsMarkedForUpdate = entity.IsMarkedForUpdate != 0,
                IsMarkedForDeletion = entity.IsMarkedForDeletion != 0,
                
                IsAssignmentTarget = entity.IsAssignmentTarget != 0,
                IsExplicitCallStatement = entity.IsExplicitCallStatement != 0,
                IsExplicitLetAssignment = entity.IsExplicitLetAssignment != 0,
                IsReDim = entity.IsReDim != 0,
                IsArrayAccess = entity.IsArrayAccess != 0,
                IsProcedureCoercion = entity.IsProcedureCoercion != 0,
                IsIndexedDefaultMemberAccess = entity.IsIndexedDefaultMemberAccess != 0,
                IsNonIndexedDefaultMemberAccess = entity.IsNonIndexedDefaultMemberAccess != 0,
                IsInnerRecursiveDefaultMemberAccess = entity.IsInnerRecursiveDefaultMemberAccess != 0,
                IsSetAssignment = entity.IsSetAssignment != 0,
                DefaultMemberRecursionDepth = entity.DefaultMemberRecursionDepth,
                TypeHint = entity.TypeHint,
                Annotations = entity.Annotations.Split('|'),
                LocationInfo = new LocationInfo
                {
                    DocumentLineStart = entity.DocumentLineStart,
                    DocumentLineEnd = entity.DocumentLineEnd,
                    ContextOffset = new DocumentOffset(entity.ContextStartOffset, entity.ContextEndOffset),
                    HighlightOffset = new DocumentOffset(entity.IdentifierStartOffset, entity.IdentifierEndOffset),
                },
            };

            var parent = await declarations.GetByIdAsync(entity.ParentDeclarationId);
            result.ParentDeclaration = await Declaration.FromEntityAsync(parent, declarations);

            var referenced = await declarations.GetByIdAsync(entity.ReferencedDeclarationId);
            result.ReferencedDeclaration = await Declaration.FromEntityAsync(referenced, declarations);

            if (entity.QualifyingReferenceId.HasValue)
            {
                var qualifier = await entities.GetByIdAsync(entity.QualifyingReferenceId.Value);
                result.QualifyingReference = await FromEntityAsync(qualifier, entities, declarations);
            }

            return result;
        }
    }
}
