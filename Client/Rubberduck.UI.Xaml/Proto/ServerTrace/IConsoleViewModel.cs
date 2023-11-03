using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.UI.Xaml.ServerTrace
{
    public interface ITraceSettingValue
    {
        string Name { get; }
        string Value { get; }
    }

    public interface IConsoleViewModel : INotifyPropertyChanged
    {
        ObservableCollection<IConsoleMesssageViewModel> ConsoleContent { get; }
        string SearchString { get; set; }
        bool IsTraceActive { get; set; }

        IEnumerable<ITraceSettingValue> TraceValues { get; }
        ITraceSettingValue SelectedTraceValue { get; set; }

        string Trace { get; set; }

        /// <summary>
        /// Shuts down the server.
        /// </summary>
        /// <remarks>
        /// Note, a client may restart another server instance.
        /// </remarks>
        ICommand ShutdownCommand { get; }
        /// <summary>
        /// Clears the console output window.
        /// </summary>
        ICommand ClearCommand { get; }
        ICommand CopyCommand { get; }
        ICommand SaveAsCommand { get; }
        ICommand PauseTraceCommand { get; }
        ICommand ResumeTraceCommand { get; }
        /// <summary>
        /// Sets the server trace level (off, message, verbose).
        /// </summary>
        ICommand SetTraceCommand { get; }
        //ICommand SearchCommand { get; }
    }
}
