using System;
using Rubberduck.InternalApi.Model.Abstract;

namespace Rubberduck.InternalApi.Model.Design
{
    public class DocumentTabDesignViewModel : IDocumentTabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
    }
}
