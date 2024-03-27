using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Settings.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace Rubberduck.UI.Shared.Settings
{
    public class ListSettingViewModel<T> : SettingViewModel<T[]>
    {
        public ListSettingViewModel(UIServiceHelper service, TypedRubberduckSetting<T[]> setting) : base(setting)
        {
            ListItems = setting.TypedValue;
            RemoveListSettingItemCommand = new DelegateCommand(service, ExecuteRemoveListSettingItemCommand);
        }

        private IEnumerable<T> _items = [];
        public IEnumerable<T> ListItems
        {
            get => _items;
            private set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RemoveListSettingItemCommand { get; }

        private void ExecuteRemoveListSettingItemCommand(object? parameter)
        {
            if (parameter is T value)
            {
                ListItems = _items.Except([value]);
                Value = _items.ToArray();
            }
        }
    }

    public class StringListSettingViewModel : ListSettingViewModel<string>
    {
        public StringListSettingViewModel(UIServiceHelper service, TypedRubberduckSetting<string[]> setting)
            : base(service, setting) { }
    }
}
