using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model;

public abstract record class NamedDocumentOffset
{
    protected NamedDocumentOffset(string name, DocumentOffset offset) 
    {
        Name = name;
        Offset = offset;
    }

    public string Name { get; }
    public DocumentOffset Offset { get; }
}
