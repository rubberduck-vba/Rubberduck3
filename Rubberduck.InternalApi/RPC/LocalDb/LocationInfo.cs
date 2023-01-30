using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{

    public class LocationInfo
    {
        public static LocationInfo Invalid { get; } 
            = new LocationInfo
            {
                ContextOffset = DocumentOffset.Invalid,
                IdentifierOffset = DocumentOffset.Invalid,
            };

        public DocumentOffset ContextOffset { get; set; }
        public DocumentOffset IdentifierOffset { get; set; }
    }
}
