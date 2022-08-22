using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Abstract
{
    public interface IEditorControlViewModel : INotifyPropertyChanged
    {
        string FontFamily { get; }
        string FontSize { get; }
        bool ShowLineNumbers { get; }
    }
}
