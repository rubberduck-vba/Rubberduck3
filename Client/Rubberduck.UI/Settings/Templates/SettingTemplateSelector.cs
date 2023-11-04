using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Settings.Templates
{
    internal class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BooleanSettingTemplate { get; set; }
        public DataTemplate EnumValueSettingTemplate { get; set; }
        public DataTemplate ListSettingTemplate { get; set; }
        public DataTemplate NumericSettingTemplate { get; set; }
        public DataTemplate SettingSubGroupTemplate { get; set; }
        public DataTemplate StringSettingTemplate { get; set; }
        public DataTemplate TimeSpanSettingTemplate { get; set; }
        public DataTemplate UriSettingTemplate { get; set; }

        private Dictionary<SettingDataType, DataTemplate> TemplateMap => new()
        {
            [SettingDataType.BooleanSetting] = BooleanSettingTemplate,
            [SettingDataType.EnumValueSetting] = EnumValueSettingTemplate,
            [SettingDataType.ListSetting] = ListSettingTemplate,
            [SettingDataType.NumericSetting] = NumericSettingTemplate,
            [SettingDataType.SettingGroup] = SettingSubGroupTemplate,
            [SettingDataType.TextSetting] = StringSettingTemplate,
            [SettingDataType.TimeSpanSetting] = TimeSpanSettingTemplate,
            [SettingDataType.UriSetting] = UriSettingTemplate,
        };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ISettingViewModel setting)
            {
                if (TemplateMap.TryGetValue(setting.SettingDataType, out var template))
                {
                    return template;
                }
            }
            return null!;
        }
    }
}
