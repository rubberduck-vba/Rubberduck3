using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Services;

namespace Rubberduck.Editor.Shell
{
    public class WindowChromeViewModel : ViewModelBase, IWindowChromeViewModel
    {
        public WindowChromeViewModel(UIServiceHelper service)
        {
            _extendWindowChrome = service.Settings.EditorSettings.ExtendWindowChrome;
            service.SettingsProvider.SettingsChanged += OnSettingsChanged;
        }

        private bool _extendWindowChrome;
        public bool ExtendWindowChrome
        {
            get => _extendWindowChrome;
            set
            {
                if (_extendWindowChrome != value)
                {
                    _extendWindowChrome = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowChromeCaptionBar { get; set; }

        public bool CanMaximize { get; set; }
        public bool CanMinimize { get; set; }

        private void OnSettingsChanged(object? sender, SettingsChangedEventArgs<RubberduckSettings> e)
        {
            var newValue = e.NewValue.EditorSettings.ExtendWindowChrome;
            if (newValue != e.OldValue?.EditorSettings.ExtendWindowChrome)
            {
                ExtendWindowChrome = newValue;
            }
        }
    }
}
