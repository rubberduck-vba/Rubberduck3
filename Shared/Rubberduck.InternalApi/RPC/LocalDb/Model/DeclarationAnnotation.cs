namespace Rubberduck.InternalApi.RPC.DataServer
{

    public class DeclarationAnnotation
    {
        public int Id { get; set; }
        public int DeclarationId { get; set; }
        public string AnnotationName { get; set; }
        public string AnnotationArgs { get; set; }

        public LocationInfo LocationInfo { get; set; }
    }
}
