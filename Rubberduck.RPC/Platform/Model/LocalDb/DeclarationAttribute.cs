namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class DeclarationAttribute : DbEntity
    {
        public int DeclarationId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValues { get; set; }

        public int? ContextStartOffset { get; set; }
        public int? ContextEndOffset { get; set; }
        public int? IdentifierStartOffset { get; set; }
        public int? IdentifierEndOffset { get; set; }
    }
}
