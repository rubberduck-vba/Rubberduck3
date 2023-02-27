namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public abstract class AnnotationBase : DbEntity
    {
        public string AnnotationName { get; set; }
        public string AnnotationArgs { get; set; }

        public int? ContextStartOffset { get; set; }
        public int? ContextEndOffset { get; set; }
        public int? IdentifierStartOffset { get; set; }
        public int? IdentifierEndOffset { get; set; }
    }
}
