using System;
using System.Text;

namespace Rubberduck.InternalApi.Extensions;

public static class MarkdownExtensions
{
    public static string ToMarkdownText(this Exception exception)
    {
        var builder = new StringBuilder();

        builder.AppendLine(exception.GetType().Name.ToHeadingMarkdownText(hLevel:2));
        builder.AppendLine(exception.Message);
        builder.AppendLine(exception.ToString().ToCodeBlockMarkdownText(language:string.Empty));

        return builder.ToString();
    }

    public static string ToHeadingMarkdownText(this string value, int hLevel = 1)
        => $"{new string('#', hLevel)} {value}";
    public static string ToBoldMarkdownText(this string value) 
        => $"**{value}**";
    public static string ToCodeBlockMarkdownText(this string value, string language = "vb")
        => $"```{language}{Environment.NewLine}{value}{Environment.NewLine}```";
}
