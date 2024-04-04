using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.LanguageServer.Diagnostics;
using Rubberduck.Resources.v3;
using Rubberduck.UI.Shared.Settings.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.UI.Shared.Settings
{
    public class DiagnosticsSettingsViewModel : SettingGroupViewModel
    {
        public DiagnosticsSettingsViewModel(TypedSettingGroup settingGroup, IEnumerable<ISettingViewModel> items) 
            : base(settingGroup, items)
        {
        }

    }

    public class DiagnosticSettingsViewModel : SettingGroupViewModel
    {
        public DiagnosticSettingsViewModel(DiagnosticSetting settingGroup, IEnumerable<ISettingViewModel> items) 
            : base(settingGroup, items.Where(e => e is not DiagnosticSeveritySettingViewModel))
        {
            Code = settingGroup.Key;
            Severity = items.OfType<DiagnosticSeveritySettingViewModel>().SingleOrDefault();
            
            SettingGroupKey = settingGroup.Key;
            SettingGroupName = settingGroup.LocalizedName;
        }

        public string Code { get; init; }
        public DiagnosticSeveritySettingViewModel Severity { get; init; }
        public string SettingGroupName { get; init; }
        public override string Name => $"[{Code}]: {SettingsUI.ResourceManager.GetString($"{SettingGroupKey}_Title") ?? $"[missing key:{SettingGroupKey}_Title]"}";
    }

    public class DiagnosticSeveritySettingViewModel : EnumValueSettingViewModel<DiagnosticSeverity>
    {
        public DiagnosticSeveritySettingViewModel(TypedRubberduckSetting<DiagnosticSeverity> setting) : base(setting)
        {
        }
    }
}
