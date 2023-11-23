using System;

namespace Rubberduck.UI.Command
{
    public interface IBrowseSelectionModel
    {
        Uri RootUri { get; set; }
        string Title { get; set; }
        string Selection { get; set; }
    }

    public interface IBrowseFolderModel : IBrowseSelectionModel { }

    public interface IBrowseFileModel : IBrowseSelectionModel
    {
        string DefaultFileExtension { get; set; }
        string Filter { get; set; }
    }

    public record class BrowseFileModel : IBrowseFileModel
    {
        public string DefaultFileExtension { get; set; }
        public string Filter { get; set; }
        public Uri RootUri { get; set; }
        public string Title { get; set; }
        public string Selection { get; set; }
    }
}
