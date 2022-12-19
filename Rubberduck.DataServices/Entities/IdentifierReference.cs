using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model.RPC;
using Rubberduck.InternalApi.Model;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class IdentifierReference : DbEntity
    {
        public int IsMarkedForDeletion { get; set; }
        public int IsMarkedForUpdate { get; set; }

        public int ReferencedDeclarationId { get; set; }
        public int ParentDeclarationId { get; set; }
        public int? QualifyingReferenceId { get; set; }
        public int IsAssignmentTarget { get; set; }
        public int IsExplicitCallStatement { get; set; }
        public int IsExplicitLetAssignment { get; set; }
        public int IsSetAssignment { get; set; }
        public int IsReDim { get; set; }
        public int IsArrayAccess { get; set; }
        public int IsProcedureCoercion { get; set; }
        public int IsIndexedDefaultMemberAccess { get; set; }
        public int IsNonIndexedDefaultMemberAccess { get; set; }
        public int IsInnerRecursiveDefaultMemberAccess { get; set; }
        public int? DefaultMemberRecursionDepth { get; set; }
        public string TypeHint { get; set; }
        public string Annotations { get; set; }
        
        public int DocumentLineStart { get; set; }
        public int DocumentLineEnd { get; set; }
        
        public int ContextStartOffset { get; set; }
        public int ContextEndOffset { get; set; }

        public int IdentifierStartOffset { get; set; }
        public int IdentifierEndOffset { get; set; }

        internal static IdentifierReference FromModel(InternalApi.Model.RPC.IdentifierReference model)
        {
            return new IdentifierReference
            {
                Id = model.Id,
                IsMarkedForUpdate = model.IsMarkedForUpdate ? 1 : 0,
                IsMarkedForDeletion = model.IsMarkedForDeletion ? 1 : 0,

                IsAssignmentTarget = model.IsAssignmentTarget ? 1 : 0,
                IsExplicitCallStatement = model.IsExplicitCallStatement ? 1 : 0,
                IsExplicitLetAssignment = model.IsExplicitLetAssignment ? 1 : 0,
                IsSetAssignment = model.IsSetAssignment ? 1 : 0,
                IsReDim = model.IsReDim ? 1 : 0,
                IsArrayAccess = model.IsArrayAccess ? 1 : 0,
                IsProcedureCoercion = model.IsProcedureCoercion ? 1 : 0,
                IsIndexedDefaultMemberAccess = model.IsIndexedDefaultMemberAccess ? 1 : 0,
                IsNonIndexedDefaultMemberAccess = model.IsNonIndexedDefaultMemberAccess ? 1 : 0,
                IsInnerRecursiveDefaultMemberAccess = model.IsInnerRecursiveDefaultMemberAccess ? 1 : 0,
                DefaultMemberRecursionDepth = model.DefaultMemberRecursionDepth,
                TypeHint = model.TypeHint,
                Annotations = string.Join("|", model.Annotations),

                DocumentLineStart = model.LocationInfo.DocumentLineStart,
                DocumentLineEnd = model.LocationInfo.DocumentLineEnd,
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                IdentifierStartOffset = model.LocationInfo.HighlightOffset.Start,
                IdentifierEndOffset = model.LocationInfo.HighlightOffset.End,

                ParentDeclarationId = model.ParentDeclaration.Id,
                ReferencedDeclarationId = model.ReferencedDeclaration.Id,
                QualifyingReferenceId = model.QualifyingReference?.Id,
            };
        }

        internal async Task<InternalApi.Model.RPC.IdentifierReference> ToModelAsync(Repository<IdentifierReference> entities, Repository<Declaration> declarations)
        {
            var result = new InternalApi.Model.RPC.IdentifierReference
            {
                Id = Id,
                IsMarkedForUpdate = IsMarkedForUpdate != 0,
                IsMarkedForDeletion = IsMarkedForDeletion != 0,

                IsAssignmentTarget = IsAssignmentTarget != 0,
                IsExplicitCallStatement = IsExplicitCallStatement != 0,
                IsExplicitLetAssignment = IsExplicitLetAssignment != 0,
                IsReDim = IsReDim != 0,
                IsArrayAccess = IsArrayAccess != 0,
                IsProcedureCoercion = IsProcedureCoercion != 0,
                IsIndexedDefaultMemberAccess = IsIndexedDefaultMemberAccess != 0,
                IsNonIndexedDefaultMemberAccess = IsNonIndexedDefaultMemberAccess != 0,
                IsInnerRecursiveDefaultMemberAccess = IsInnerRecursiveDefaultMemberAccess != 0,
                IsSetAssignment = IsSetAssignment != 0,
                DefaultMemberRecursionDepth = DefaultMemberRecursionDepth,
                TypeHint = TypeHint,
                Annotations = Annotations.Split('|'),
                LocationInfo = new LocationInfo
                {
                    DocumentLineStart = DocumentLineStart,
                    DocumentLineEnd = DocumentLineEnd,
                    ContextOffset = new DocumentOffset(ContextStartOffset, ContextEndOffset),
                    HighlightOffset = new DocumentOffset(IdentifierStartOffset, IdentifierEndOffset),
                },
            };

            var parent = await declarations.GetByIdAsync(ParentDeclarationId);
            result.ParentDeclaration = await parent.ToModelAsync(declarations);

            var referenced = await declarations.GetByIdAsync(ReferencedDeclarationId);
            result.ReferencedDeclaration = await referenced.ToModelAsync(declarations);

            if (QualifyingReferenceId.HasValue)
            {
                var qualifier = await entities.GetByIdAsync(QualifyingReferenceId.Value);
                result.QualifyingReference = await qualifier.ToModelAsync(entities, declarations);
            }

            return result;
        }
    }
}
