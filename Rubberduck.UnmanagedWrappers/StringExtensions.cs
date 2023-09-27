using System;
using System.IO;

namespace Rubberduck.Unmanaged
{
    public static class StringExtensions
    {
        public static string[] ToCodeLines(this string source)
        {
            return source?.Replace("\r", string.Empty).Split('\n') ?? new string[] { };
        }

        public static Stream ToStream(this string source)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream); // do not dispose the writer http://stackoverflow.com/a/1879470/1188513
            writer.Write(source);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
