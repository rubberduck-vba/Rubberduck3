using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Settings.ViewModels.Abstract;
using System;

namespace Rubberduck.UI.Settings.ViewModels
{
    public class UriSettingViewModel : SettingViewModel<Uri>, IBrowseFolderModel
    {
        public UriSettingViewModel(UriRubberduckSetting setting) : base(setting)
        {
        }

        public Uri RootUri
        {
            get => Value;
            set => Value = value;
        }
        public string Title { get; set; }
        public string Selection { get; set; }
    }
}
