using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public class DiagnosticSeveritySettingViewModel : EnumValueSettingViewModel<DiagnosticSeverity>
    {
        public DiagnosticSeveritySettingViewModel(TypedRubberduckSetting<DiagnosticSeverity> setting) : base(setting)
        {
        }
    }
}
