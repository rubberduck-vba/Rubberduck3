using System;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class TypedSettingGroup : TypedRubberduckSetting<RubberduckSetting[]>
    {
        public TSetting GetSetting<TSetting>() where TSetting : RubberduckSetting => ((RubberduckSetting[])Value).OfType<TSetting>().Single();
        public RubberduckSetting GetSetting(Type type) => ((RubberduckSetting[])Value).Single(e => e.GetType() == type);

        protected TypedSettingGroup()
        {
            SettingDataType = SettingDataType.SettingGroup;
        }
    }

    public abstract record class EnumSettingGroup<TEnum> : TypedRubberduckSetting<BooleanRubberduckSetting[]>
        where TEnum : struct, Enum
    {
        public BooleanRubberduckSetting GetSetting(TEnum key) => ((BooleanRubberduckSetting[])Value).Single(e => e.Key == key.ToString());

        protected EnumSettingGroup()
        {
            SettingDataType = SettingDataType.SettingGroup;
        }
    }
}
