using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model
{
    public readonly struct QualifiedDocumentOffset
    {
        public QualifiedDocumentOffset(QualifiedModuleName module, DocumentOffset offset)
        {
            QualifiedModuleName = module;
            Offset = offset;
        }

        public QualifiedModuleName QualifiedModuleName { get; }
        public DocumentOffset Offset { get; }

        // TODO implement IEquatable, IComparable, ...
    }

    public readonly struct DocumentOffset
    {
        public static DocumentOffset Invalid = new DocumentOffset(0, -1);

        public DocumentOffset(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public int Start { get; }
        public int End { get; }
        public int Length { get; }

        public bool Contains(DocumentOffset other)
        {
            return other.Start >= Start && other.End <= End;
        }
    }
}
