using System;

namespace Rubberduck.SettingsProvider
{
    public class SettingsChangedEventArgs<TSettings> : EventArgs
    {
        public SettingsChangedEventArgs(TSettings oldValue, TSettings currentValue, Guid token)
        {
            OldValue = oldValue;
            Value = currentValue;
            Token = token;
        }

        public Guid Token { get; }
        public TSettings OldValue { get; }
        public TSettings Value { get; }
    }
}
