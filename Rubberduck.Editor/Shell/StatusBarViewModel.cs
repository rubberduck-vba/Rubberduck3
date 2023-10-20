using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.Editor.Shell
{
    public class StatusBarViewModel : ViewModelBase
    {
        private ServerConnectionState _serverConnectionState = ServerConnectionState.Disconnected;
        ServerConnectionState ServerConnectionState
        {
            get => _serverConnectionState;
            set
            {
                if (_serverConnectionState != value)
                {
                    _serverConnectionState = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowDocumentStatusItems { get; set; } = false;

        private string _serverStateText = ServerConnectionState.Disconnected.ToString(); // TODO localize
        public string ServerStateText
        {
            get => _serverStateText;
            set
            {
                if (value != _serverStateText)
                {
                    _serverStateText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
