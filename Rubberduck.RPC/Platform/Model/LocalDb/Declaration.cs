namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class Declaration : DbEntity
    {
        public long DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public int IsUserDefined { get; set; }
        public int IsArray { get; set; }
        public string AsTypeName { get; set; }
        public int? AsTypeDeclarationId { get; set; }
        public string TypeHint { get; set; }
        public int? ParentDeclarationId { get; set; }
        public string DocString { get; set; }

        public int? ContextStartOffset { get; set; }
        public int? ContextEndOffset { get; set; }
        public int? IdentifierStartOffset { get; set; }
        public int? IdentifierEndOffset { get; set; }
    }
}
