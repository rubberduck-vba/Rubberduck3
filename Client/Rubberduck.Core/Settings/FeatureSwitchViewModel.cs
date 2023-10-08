using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Core.Settings
{
    public class FeatureSwitchViewModel : ISettingsViewModel<FeatureSwitch>
    {
        private readonly FeatureSwitch _settings;

        public FeatureSwitchViewModel(FeatureSwitch settings)
        {
            _settings = settings;

            NameResourceKey = settings.NameResourceKey;
            IsEnabled = settings.IsEnabled;
        }

        public string NameResourceKey { get; set; }
        public bool IsEnabled { get; set; }

        public FeatureSwitch ToSettings()
        {
            return new FeatureSwitch
            {
                NameResourceKey = this.NameResourceKey,
                IsEnabled = this.IsEnabled,
            };
        }
    }
}
