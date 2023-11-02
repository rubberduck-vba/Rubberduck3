using System;

namespace Rubberduck.InternalApi.Settings
{
    public class SettingsChangedEventArgs<TSettings> : EventArgs
    {
        public SettingsChangedEventArgs(TSettings oldValue, TSettings newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public TSettings OldValue { get; }
        public TSettings NewValue { get; }
    }
}
