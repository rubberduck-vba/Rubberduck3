using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract class TypedSettingGroup : TypedRubberduckSetting<RubberduckSetting[]>
    {
        private IReadOnlyDictionary<Type, RubberduckSetting> _map => TypedValue.ToDictionary(e => e.GetType());
        public TSetting GetSetting<TSetting>() where TSetting : RubberduckSetting => ((RubberduckSetting[])Value).OfType<TSetting>().Single();
        public RubberduckSetting GetSetting(Type type) => ((RubberduckSetting[])Value).Single(e => e.GetType() == type);

        protected TypedSettingGroup()
        {
            SettingDataType = SettingDataType.SettingGroup;
        }
    }

    public abstract class EnumSettingGroup<TEnum> : TypedRubberduckSetting<RubberduckSetting[]>
        where TEnum : struct, Enum
    {
        private IReadOnlyDictionary<TEnum, RubberduckSetting> _map => TypedValue.ToDictionary(e => Enum.Parse<TEnum>(e.Key));

        public RubberduckSetting GetSetting(TEnum key) => ((RubberduckSetting[])Value).Single(e => e.Key == key.ToString());
    }
}
