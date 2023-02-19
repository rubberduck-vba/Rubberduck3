using System;

namespace Rubberduck.InternalApi.Common
{
    /// <summary>
    /// Disables a feature
    /// </summary>
    public class DisabledAttribute : Attribute
    {

    }

    /// <summary>
    /// Marks a feature as experimental (user must explicitly enable the feature)
    /// </summary>
    public class ExperimentalAttribute : Attribute 
    {
        public ExperimentalAttribute(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; }
    }
}
