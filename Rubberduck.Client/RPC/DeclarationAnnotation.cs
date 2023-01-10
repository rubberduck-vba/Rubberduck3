using ProtoBuf;

namespace Rubberduck.Client.RPC
{
    [ProtoContract]
    public class DeclarationAnnotation
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public int DeclarationId { get; set; }
        [ProtoMember(3)]
        public string AnnotationName { get; set; }
        [ProtoMember(4)]
        public string AnnotationArgs { get; set; }

        [ProtoMember(5)]
        public LocationInfo LocationInfo { get; set; }
    }
}
