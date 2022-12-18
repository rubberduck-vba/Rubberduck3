using ProtoBuf;
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
    }
}
