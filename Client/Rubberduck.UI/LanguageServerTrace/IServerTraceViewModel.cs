using Rubberduck.UI.Windows;
using System;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.LanguageServerTrace
{
    public interface IServerTraceViewModel : IToolWindowViewModel
    {
        string ConsoleContent { get; set; }
    }
}
