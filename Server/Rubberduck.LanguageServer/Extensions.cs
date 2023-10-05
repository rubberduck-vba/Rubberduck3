using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Rubberduck.LanguageServer
{
    public static class Extensions
    {
        public static TraceLevel ToTraceLevel(this InitializeTrace value)
        {
            switch (value)
            {
                case InitializeTrace.Off:
                    return TraceLevel.Off;
                case InitializeTrace.Verbose:
                    return TraceLevel.Verbose;
                default:
                    return TraceLevel.Info; // == ServerTraceLevel.Message
            }
        }

        public static CultureInfo FromLocale(this CultureInfo _, string? locale)
        {
            try
            {
                return CultureInfo.GetCultureInfo(locale);
            }
            catch
            {
                return CultureInfo.InvariantCulture;
            }
        }

        public static Container<T> ToContainer<T>(this IEnumerable<T> source)
        {
            return new Container<T>(source);
        }
    }
}