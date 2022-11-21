using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface IEditorShellViewModel : INotifyPropertyChanged, IStatusUpdate
    {
        ObservableCollection<ICodePaneViewModel> Documents { get; }
    }
}
