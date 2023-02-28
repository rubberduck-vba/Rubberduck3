using System;

namespace Rubberduck.RPC.Platform.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a command is given an unexpected value for an enum argument.
    /// </summary>
    public class InvalidEnumValueException<TEnum, TValue> : ArgumentException
    {
        public InvalidEnumValueException(string message, TValue value)
            : base(message)
        {
            Value = value;
        }

        public Type Type { get; } = typeof(TEnum);
        public TValue Value { get; }
    }
}
