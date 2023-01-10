using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    [ProtoContract]
    public class LocationInfo
    {
        public static LocationInfo Invalid { get; } 
            = new LocationInfo
            {
                ContextOffset = DocumentOffset.Invalid,
                IdentifierOffset = DocumentOffset.Invalid,
            };

        [ProtoMember(1)]
        public DocumentOffset ContextOffset { get; set; }
        [ProtoMember(2)]
        public DocumentOffset IdentifierOffset { get; set; }
    }
}
