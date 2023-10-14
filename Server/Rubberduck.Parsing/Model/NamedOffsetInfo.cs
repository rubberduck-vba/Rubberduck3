using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model;

public abstract class NamedOffsetInfo
{
    protected NamedOffsetInfo(string name, DocumentOffset offset) 
    {
        Name = name;
        Offset = offset;
    }

    public string Name { get; }
    public DocumentOffset Offset { get; }
}
