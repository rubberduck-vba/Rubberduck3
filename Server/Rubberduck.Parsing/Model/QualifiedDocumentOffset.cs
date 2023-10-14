using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.Model;

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
