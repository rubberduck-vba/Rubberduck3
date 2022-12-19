using ProtoBuf;
using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model.RPC;
using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class Declaration : DbEntity
    {
        public int IsMarkedForDeletion { get; set; }
        public int IsMarkedForUpdate { get; set; }

        public long DeclarationType { get; set; }
        public int IsArray { get; set; }
        public string AsTypeName { get; set; }
        public int? AsTypeDeclarationId { get; set; }
        public string TypeHint { get; set; }
        public string IdentifierName { get; set; }
        public int IsUserDefined { get; set; }
        public int? ParentDeclarationId { get; set; }
        public string DocString { get; set; }
        public string Annotations { get; set; }
        public string Attributes { get; set; }
        public int? DocumentLineStart { get; set; }
        public int? DocumentLineEnd { get; set; }
        public int? ContextStartOffset { get; set; }
        public int? ContextEndOffset { get; set; }
        public int? HighlightStartOffset { get; set; }
        public int? HighlightEndOffset { get; set; }

        internal static Declaration FromModel(InternalApi.Model.RPC.Declaration model)
        {
            return new Declaration
            {
                Id = model.Id,
                IsMarkedForUpdate = model.IsMarkedForUpdate ? 1 : 0,
                IsMarkedForDeletion = model.IsMarkedForDeletion ? 1 : 0,

                Annotations = string.Join("|", model.Annotations),
                AsTypeDeclarationId = model.AsTypeDeclaration?.Id,
                AsTypeName = model.AsTypeName,
                Attributes = string.Join("|", model.Attributes),
                ContextStartOffset = model.LocationInfo.ContextOffset.Start,
                ContextEndOffset = model.LocationInfo.ContextOffset.End,
                DeclarationType = (long)model.DeclarationType,
                DocString = model.DocString,
                DocumentLineEnd = model.LocationInfo.DocumentLineEnd,
                DocumentLineStart = model.LocationInfo.DocumentLineStart,
                HighlightStartOffset = model.LocationInfo.HighlightOffset.Start,
                HighlightEndOffset = model.LocationInfo.HighlightOffset.End,
                IdentifierName = model.IdentifierName,
                IsArray = model.IsArray ? 1 : 0,
                IsUserDefined = model.IsUserDefined ? 1 : 0,
                ParentDeclarationId = model.ParentDeclaration?.Id,
                TypeHint = model.TypeHint,
            };
        }

        internal async Task<InternalApi.Model.RPC.Declaration> ToModelAsync(Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Declaration
            {
                Id = Id,
                IsMarkedForUpdate = IsMarkedForUpdate != 0,
                IsMarkedForDeletion = IsMarkedForDeletion != 0,

                AsTypeName = AsTypeName,
                DeclarationType = (DeclarationType)DeclarationType,
                DocString = DocString,
                IdentifierName = IdentifierName,
                IsArray = IsArray != 0,
                IsUserDefined = IsUserDefined != 0,
                TypeHint = TypeHint,

                Annotations = Annotations.Split('|'),
                Attributes = Attributes.Split('|'),
            };

            result.LocationInfo = result.IsUserDefined
                ? new LocationInfo
                {
                    DocumentLineStart = DocumentLineStart ?? 0,
                    DocumentLineEnd = DocumentLineEnd ?? 0,
                    ContextOffset = new DocumentOffset(ContextStartOffset ?? 0, ContextEndOffset ?? -1),
                    HighlightOffset = new DocumentOffset(HighlightStartOffset ?? 0, HighlightEndOffset ?? -1)
                }
                : null;

            if (ParentDeclarationId.HasValue)
            {
                var parent = await repository.GetByIdAsync(ParentDeclarationId.Value);
                result.ParentDeclaration = await parent.ToModelAsync(repository);
            }

            if (AsTypeDeclarationId.HasValue)
            {
                var asType = await repository.GetByIdAsync(AsTypeDeclarationId.Value);
                result.AsTypeDeclaration = await asType.ToModelAsync(repository);
            }

            return result;
        }
    }
}
