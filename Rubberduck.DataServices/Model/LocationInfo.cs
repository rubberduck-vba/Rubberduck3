using Rubberduck.InternalApi.Model;

namespace Rubberduck.DataServices.Model
{
    public class LocationInfo
    {
        public int DocumentLineStart { get; set; }
        public int DocumentLineEnd { get; set; }
        public DocumentOffset ContextOffset { get; set; }
        public DocumentOffset HighlightOffset { get; set; }
    }
}
