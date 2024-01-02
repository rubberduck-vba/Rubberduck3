using Rubberduck.UI.Shell.Document;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
//using Rubberduck.Parsing.Annotations.Concrete;
//using Rubberduck.Parsing.Symbols;
//using Rubberduck.Resources.CodeExplorer;

namespace Rubberduck.UI.Converters
{
    public class SupportedDocumentTypeToIconConverter : ImageSourceConverter
    {
        private static readonly IDictionary<SupportedDocumentType, ImageSource> Mapping = new Dictionary<SupportedDocumentType, ImageSource>
        {
            [SupportedDocumentType.SourceFile] = null,
            [SupportedDocumentType.MarkdownDocument] = null,
            [SupportedDocumentType.TextDocument] = null,
            [SupportedDocumentType.ProjectFile] = null,
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var documentType = (SupportedDocumentType)value;
            if (Mapping.TryGetValue(documentType, out var mappedImageSource))
            {
                return mappedImageSource;
            }

            return null!;
        }
    }
}