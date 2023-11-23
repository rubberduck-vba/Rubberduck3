using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI;

namespace Rubberduck.Editor.Shell
{
    public class StatusBarViewModel : ViewModelBase, IStatusBarViewModel
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

        public int DocumentLines { get; set; }
        public int DocumentLength { get; set; }
        public int CaretOffset { get; set; }
        public int CaretLine { get; set; }
        public int CaretColumn { get; set; }
        public int IssuesCount { get; set; }
        ServerConnectionState IStatusBarViewModel.ServerConnectionState { get; set; }
        public int ProgressValue { get; set; }
        public int ProgressMaxValue { get; set; }
    }
}
