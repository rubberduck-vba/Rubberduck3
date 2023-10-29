using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class TypedSettingGroup : TypedRubberduckSetting<RubberduckSetting[]>
    {
        protected TypedSettingGroup(string name, IEnumerable<RubberduckSetting> settings, IEnumerable<RubberduckSetting> defaults)
            : base(name, settings.ToArray(), SettingDataType.ObjectSetting, defaults.ToArray())
        {
            _map = TypedValue.ToDictionary(e => e.GetType());
        }

        private readonly IDictionary<Type, RubberduckSetting> _map;
        public TSetting GetSetting<TSetting>() where TSetting : RubberduckSetting => (TSetting)_map[typeof(TSetting)];
        public RubberduckSetting GetSetting(Type type) => _map[type];
    }

    public abstract record class EnumSettingGroup<TEnum> : TypedRubberduckSetting<RubberduckSetting[]>
        where TEnum : struct, Enum
    {
        protected EnumSettingGroup(string name, IEnumerable<RubberduckSetting> settings, IEnumerable<RubberduckSetting> defaults)
            : base(name, settings.ToArray(), SettingDataType.ObjectSetting, defaults.ToArray())
        {
            _map = ((RubberduckSetting[])settings).ToDictionary(e => Enum.Parse<TEnum>(e.Key));
        }

        private readonly IDictionary<TEnum, RubberduckSetting> _map;
        public RubberduckSetting GetSetting(TEnum key) => _map[key];
    }
}
