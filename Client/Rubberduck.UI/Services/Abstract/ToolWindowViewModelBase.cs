using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Services.Abstract
{
    public abstract class ToolWindowViewModelBase : WindowViewModel, IToolWindowViewModel
    {
        protected ToolWindowViewModelBase(DockingLocation location, 
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
            : base(showSettingsCommand, closeToolWindowCommand)
        {
            DockingLocation = location;
            ShowPinButton = location != DockingLocation.None;
            Header = Title;
        }

        private DockingLocation _location;
        public DockingLocation DockingLocation
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand UndockToolTabCommand { get; }


        private object _header;
        public virtual object Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _textContent;
        public string TextContent
        {
            get => _textContent;
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _contentControl;
        public object ContentControl
        {
            get => _contentControl;
            set
            {
                if (_contentControl != value)
                {
                    _contentControl = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
