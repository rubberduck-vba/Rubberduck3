using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;

namespace Rubberduck.ServerPlatform
{
    public static class Extensions
    {
        public static TraceLevel ToTraceLevel(this InitializeTrace value)
        {
            return value switch
            {
                InitializeTrace.Off => TraceLevel.Off,
                InitializeTrace.Verbose => TraceLevel.Verbose,
                _ => TraceLevel.Info,// == ServerTraceLevel.Message
            };
        }

        public static CultureInfo FromLocale(this CultureInfo _, string locale)
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