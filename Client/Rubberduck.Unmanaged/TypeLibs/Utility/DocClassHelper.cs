﻿using System.Text;
using Rubberduck.Unmanaged.TypeLibs.Abstract;

namespace Rubberduck.Unmanaged.TypeLibs.Utility
{
    /// <summary>
    /// Extension to StringBuilder to allow adding text line by line.
    /// </summary>
    internal class StringLineBuilder
    {
        private readonly StringBuilder _document = new();

        public override string ToString() => _document.ToString();

        public void AppendLine(string value = "")
            => _document.Append(value + "\r\n");

        public void AppendLineNoNullChars(string value)
            => AppendLine(value.Replace("\0", string.Empty));
    }

    /// <summary>
    /// An enumeration used for identifying the type of a VBA document class
    /// </summary>
    public enum DocClassType
    {
        Unrecognized = 0,
        ExcelWorkbook = 1,
        ExcelWorksheet = 2,
        AccessForm = 3,
        AccessReport = 4,
    }

    /// <summary>
    /// A helper class for providing a static array of known VBA document class types
    /// </summary>
    internal static class DocClassHelper
    {
        /// <summary>
        /// A class for holding known document class types used in VBA hosts, and their corresponding interface progIds
        /// </summary>
        public struct KnownDocType
        {
            public string DocTypeInterfaceProgId;
            public DocClassType DocType;

            public KnownDocType(string docTypeInterfaceProgId, DocClassType docType)
            {
                DocTypeInterfaceProgId = docTypeInterfaceProgId;
                DocType = docType;
            }
        }

        public static KnownDocType[] KnownDocumentInterfaces =
        {
            new("Excel._Workbook",     DocClassType.ExcelWorkbook),
            new("Excel._Worksheet",    DocClassType.ExcelWorksheet),
            new("Access._Form",        DocClassType.AccessForm),
            new("Access._Form2",       DocClassType.AccessForm),
            new("Access._Form3",       DocClassType.AccessForm),
            new("Access._Report",      DocClassType.AccessReport),
            new("Access._Report2",     DocClassType.AccessReport),
            new("Access._Report3",     DocClassType.AccessReport),
        };

        // string array of the above progIDs, created once at runtime
        public static string[] KnownDocumentInterfaceProgIds;

        static DocClassHelper()
        {
            var index = 0;
            KnownDocumentInterfaceProgIds = new string[KnownDocumentInterfaces.Length];
            foreach (var knownDocClass in KnownDocumentInterfaces)
            {
                KnownDocumentInterfaceProgIds[index++] = knownDocClass.DocTypeInterfaceProgId;
            }
        }

        /// <summary>
        /// Determines the document class type of a VBA class.  See <see cref="DocClassHelper" />
        /// </summary>
        /// <returns>the identified document class type, or <see cref="DocClassType.Unrecognized" /></returns>
        public static DocClassType DetermineDocumentClassType(ITypeInfoWrapper rootInterface)
        {
            return rootInterface.ImplementedInterfaces
                .DoesImplement(DocClassHelper.KnownDocumentInterfaceProgIds, out var matchId)
                ? DocClassHelper.KnownDocumentInterfaces[matchId].DocType
                : DocClassType.Unrecognized;
        }
    }
}
