using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;
using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
using Rubberduck.InternalApi.Settings.Model.LanguageServer.Diagnostics;
using Rubberduck.InternalApi.Settings.Model.Logging;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using Rubberduck.InternalApi.Settings.Model.TelemetryServer;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shared.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.UI.Services.Settings
{
    public class SettingViewModelFactory : ISettingViewModelFactory
    {
        private readonly UIServiceHelper _service;

        public SettingViewModelFactory(UIServiceHelper service)
        {
            _service = service;
        }

        public ISettingViewModel CreateViewModel(BooleanRubberduckSetting setting) => new BooleanSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(NumericRubberduckSetting setting) => new NumericSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(StringRubberduckSetting setting) => new StringSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(UriRubberduckSetting setting) => new UriSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(LogLevelSetting setting) => new LogLevelSettingViewModel(setting);
        public ISettingViewModel CreateViewModel(DiagnosticSeveritySetting setting) => new DiagnosticSeveritySettingViewModel(setting);
        public ISettingViewModel CreateViewModel(DefaultToolWindowLocationSetting setting) => new DefaultToolWindowLocationSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(TraceLevelSetting setting) => new MessageTraceLevelSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(ServerTransportTypeSetting setting) => new ServerTransportTypeSettingViewModel(setting);

        public ISettingViewModel CreateViewModel(ServerMessageModeSetting setting) => new ServerMessageModeSettingViewModel(setting);
        public ISettingViewModel CreateViewModel(TypedRubberduckSetting<TimeSpan> setting) => new TimeSpanSettingViewModel(setting);
        public ISettingViewModel CreateViewModel(TypedRubberduckSetting<string[]> setting) => new ListSettingViewModel(_service, setting);

        public ISettingGroupViewModel CreateViewModel(TypedRubberduckSetting<BooleanRubberduckSetting[]> settingGroup)
        {
            var items = settingGroup.TypedValue?.Select(e => CreateViewModel(e)).ToList() ?? Enumerable.Empty<ISettingViewModel>().ToList();
            return new SettingGroupViewModel(settingGroup, items);
        }

        public ISettingGroupViewModel CreateViewModel(TypedRubberduckSetting<RubberduckSetting[]> settingGroup)
        {
            var items = settingGroup.TypedValue?.Cast<BooleanRubberduckSetting>().Select(e => CreateViewModel(e)).ToList() ?? Enumerable.Empty<ISettingViewModel>().ToList();
            return new SettingGroupViewModel(settingGroup, items);
        }

        public ISettingGroupViewModel CreateViewModel(TypedSettingGroup settingGroup)
        {
            if (settingGroup is RubberduckSettings root)
            {
                var items = root.TypedValue.OfType<TypedSettingGroup>().Select(CreateViewModel).ToList();
                return new SettingGroupViewModel(root, items);
            }
            else
            {
                return new SettingGroupViewModel(settingGroup, GetChildItems(settingGroup).ToList());
            }
        }

        private IEnumerable<ISettingViewModel> GetChildItems(TypedSettingGroup settingGroup)
        {
            foreach (var setting in settingGroup.TypedValue)
            {
                yield return GetChildItem(setting);
            }
        }

        private ISettingViewModel GetChildItem<TSetting>(TSetting setting) where TSetting : RubberduckSetting
        {
            switch (setting)
            {
                case BooleanRubberduckSetting booleanSetting:
                    return CreateViewModel(booleanSetting);
                case NumericRubberduckSetting numericSetting:
                    return CreateViewModel(numericSetting);
                case StringRubberduckSetting stringSetting:
                    return CreateViewModel(stringSetting);
                case UriRubberduckSetting uriSetting:
                    return CreateViewModel(uriSetting);
                case LogLevelSetting loglevelSetting:
                    return CreateViewModel(loglevelSetting);
                case DiagnosticSeveritySetting severitySetting:
                    return CreateViewModel(severitySetting);
                case DefaultToolWindowLocationSetting toolwindowLocationSetting:
                    return CreateViewModel(toolwindowLocationSetting);
                case TraceLevelSetting tracelevelSetting:
                    return CreateViewModel(tracelevelSetting);
                case ServerTransportTypeSetting serverTransportTypeSetting:
                    return CreateViewModel(serverTransportTypeSetting);
                case ServerMessageModeSetting serverMessageModeSetting:
                    return CreateViewModel(serverMessageModeSetting);
                case ClientHealthCheckIntervalSetting healthCheckIntervalSetting:
                    return CreateViewModel(healthCheckIntervalSetting);
                case TypedRubberduckSetting<TimeSpan> timeSpanSetting:
                    return CreateViewModel(timeSpanSetting);
                case TypedRubberduckSetting<string[]> listSetting:
                    return new StringListSettingViewModel(_service, listSetting);
                case TypedSettingGroup subGroup:
                    return CreateViewModel(subGroup);
                case MappedBoolSettingGroup mappedGroup:
                    return CreateViewModel(mappedGroup);
                default:
                    Debug.WriteLine($"**BUG** Missing case for '{setting.Key}' (data type: {setting.SettingDataType}) in SettingViewModelFactory.");
                    throw new NotSupportedException();
            }
        }
    }
}
