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

    private Dictionary<string, RubberduckSetting>? Values => TypedValue?.ToDictionary(e => e.Key);

    protected TypedSettingGroup()
    {
        SettingDataType = SettingDataType.SettingGroup;
    }

    public TypedSettingGroup WithSetting(RubberduckSetting? setting)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));

        var values = Values ?? throw new InvalidOperationException();
        values[setting.Key] = setting;
        return this with { Value = values.Values.ToArray() };
    }
    public TypedSettingGroup WithSetting<TSetting>(TSetting setting) where TSetting : RubberduckSetting
    {
        var values = Values ?? throw new InvalidOperationException();
        values[setting.Key] = setting;
        return this with { Value = values.Values.ToArray() };
    }

    public IEnumerable<RubberduckSettingNode> Flatten(TypedSettingGroup? root = default)
    {
        var result = new HashSet<RubberduckSettingNode>();
        foreach (var setting in root?.TypedValue ?? TypedValue)
        {
            var baseKey = root?.Key;
            foreach (var item in Flatten(setting, baseKey))
            {
                result.Add(new(item, $"{baseKey}.{setting.Key}"));
            }
        }
        return result;
    }

    private IEnumerable<RubberduckSettingNode> Flatten(RubberduckSetting setting, string? root = default)
    {
        var result = new HashSet<RubberduckSettingNode>();
        if (setting is TypedSettingGroup group)
        {
            foreach (var item in Flatten(group))
            {
                result.Add(item);
            }
        }
        else
        {
            result.Add(new(setting, root!));
        }
        return result;
    }
}

public abstract record class EnumSettingGroup<TEnum, TSetting> : TypedSettingGroup
    where TEnum : struct, Enum
    where TSetting : RubberduckSetting
{
    public EnumSettingGroup()
    {
    }

    public TSetting GetSetting(TEnum key) => ((TSetting[])Value).Single(e => e.Key == key.ToString());
}
