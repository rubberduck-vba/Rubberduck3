using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.ServerPlatform
{
    public class InvalidInitializeParamsException : ArgumentException
    {
        public static void ThrowIfNull(InitializeParams param, params Func<InitializeParams, (string, object?)>[] requiredValues)
        {
            foreach (var requiredValue in requiredValues)
            {
                var (property, value) = requiredValue.Invoke(param);
                if (value is null)
                {
                    throw new InvalidInitializeParamsException(property);
                }
            }
        }

        public InvalidInitializeParamsException(string property)
            : base($"Property '{property}' cannot be null") { }
    }
}