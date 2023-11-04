using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class TypedSettingGroup : TypedRubberduckSetting<RubberduckSetting[]>
    {
        public TSetting GetSetting<TSetting>() where TSetting : RubberduckSetting => ((RubberduckSetting[])Value).OfType<TSetting>().Single();
        public RubberduckSetting GetSetting(Type type) => ((RubberduckSetting[])Value).Single(e => e.GetType() == type);

        private Dictionary<Type, RubberduckSetting> Values => TypedValue.ToDictionary(e => e.GetType());

        protected TypedSettingGroup()
        {
            SettingDataType = SettingDataType.SettingGroup;
        }

        public TypedSettingGroup WithSetting(RubberduckSetting setting)
        {
            var values = Values;
            values[setting.GetType()] = setting;
            return this with { Value = values.Values };
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
