using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels.Abstract;
using System;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class UriSettingViewModel : SettingViewModel<Uri>
    {
        public UriSettingViewModel(UriRubberduckSetting setting) : base(setting)
        {
        }
    }
}
