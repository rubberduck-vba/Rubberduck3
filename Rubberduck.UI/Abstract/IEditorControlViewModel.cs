using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface IEditorSettings
    {
        string FontFamily { get; set; }
        string FontSize { get; set; }
        bool ShowLineNumbers { get; set; }
    }

    public interface IEditorShellViewModel : INotifyPropertyChanged, IStatusUpdate
    {
        ObservableCollection<IEditorTabViewModel> Documents { get; }
    }

    public interface IEditorTabViewModel : INotifyPropertyChanged, IStatusUpdate
    {
        string Title { get; set; }
        string ModuleContent { get; set; }
        IEditorSettings EditorSettings { get; set; }
    }
}
