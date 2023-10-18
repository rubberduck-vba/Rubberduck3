using System;

namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface IDocumentTabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Language { get; set; }

        public string Title { get; set; }
        public object Content { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
