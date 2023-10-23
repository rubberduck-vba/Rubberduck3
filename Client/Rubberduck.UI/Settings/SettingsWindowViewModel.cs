using Rubberduck.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.UI.Settings
{
    public class SettingsWindowViewModel : ViewModelBase, IWindowViewModel
    {
        public string Title => RubberduckUI.Settings;
    }
}
