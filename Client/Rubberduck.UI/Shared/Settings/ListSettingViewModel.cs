using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Settings.Abstract;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Rubberduck.UI.Shared.Settings
{
    public class ListSettingViewModel : SettingViewModel<string[]>
    {
        public ListSettingViewModel(UIServiceHelper service, TypedRubberduckSetting<string[]> setting) : base(setting)
        {
            ListItems = new ObservableCollection<string>(setting.TypedValue);
            RemoveListSettingItemCommand = new DelegateCommand(service, ExecuteRemoveListSettingItemCommand);
        }

        public ObservableCollection<string> ListItems { get; }

        public ICommand RemoveListSettingItemCommand { get; }

        private void ExecuteRemoveListSettingItemCommand(object? parameter)
        {
            if (parameter is string value)
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    ListItems.Remove(value);
                    Value = ListItems.ToArray();
                });
            }
        }
    }
}
