using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface IEditorControlViewModel : INotifyPropertyChanged, IStatusUpdate
    {
        string FontFamily { get; set; }
        string FontSize { get; set; }
        bool ShowLineNumbers { get; set; }
        string ModuleContent { get; set; }
    }
}
