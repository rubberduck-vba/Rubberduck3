using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Settings.Model;

public abstract record class TypedSettingGroup : TypedRubberduckSetting<RubberduckSetting[]>
{
    public TSetting? GetSetting<TSetting>() where TSetting : RubberduckSetting
    {
        return ((RubberduckSetting[])Value).OfType<TSetting>().SingleOrDefault();
    }

    public RubberduckSetting GetSetting(Type type) => ((RubberduckSetting[])Value).Single(e => e.GetType() == type);

    private Dictionary<Type, RubberduckSetting>? Values => TypedValue?.ToDictionary(e => e.GetType());

    protected TypedSettingGroup()
    {
        SettingDataType = SettingDataType.SettingGroup;
    }

    public TypedSettingGroup WithSetting(RubberduckSetting setting)
    {
        var values = Values ?? throw new InvalidOperationException();
        values[setting.GetType()] = setting;
        return this with { Value = values.Values };
    }
    public TypedSettingGroup WithSetting<TSetting>(TSetting setting) where TSetting : RubberduckSetting
    {
        var values = Values ?? throw new InvalidOperationException();
        values[typeof(TSetting)] = setting;
        return this with { Value = values.Values };
    }

    public IEnumerable<RubberduckSetting> Flatten(TypedSettingGroup? root = default)
    {
        foreach (var setting in root?.TypedValue ?? TypedValue)
        {
            foreach (var item in Flatten(setting))
            {
                yield return item;
            }
        }
    }

    private IEnumerable<RubberduckSetting> Flatten(RubberduckSetting setting)
    {
        if (setting is TypedSettingGroup group)
        {
            foreach (var item in Flatten(group))
            {
                yield return item;
            }
        }
        else
        {
            yield return setting;
        }
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
