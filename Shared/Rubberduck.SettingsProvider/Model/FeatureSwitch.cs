using System;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct FeatureSwitch : IEquatable<FeatureSwitch>
    {
        public string NameResourceKey { get; init; }
        public bool IsEnabled { get; init; }

        public bool Equals(FeatureSwitch other)
        {
            return string.Equals(NameResourceKey, other.NameResourceKey, StringComparison.InvariantCultureIgnoreCase)
                && IsEnabled == other.IsEnabled;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((FeatureSwitch)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NameResourceKey.ToLowerInvariant(), IsEnabled);
        }
    }

    public class FeatureSwitchKeyComparer : IEqualityComparer<FeatureSwitch>
    {
        public bool Equals(FeatureSwitch x, FeatureSwitch y)
        {
            return string.Equals(x.NameResourceKey, y.NameResourceKey, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(FeatureSwitch obj)
        {
            return HashCode.Combine(obj.NameResourceKey);
        }
    }
}
