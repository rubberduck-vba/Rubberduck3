using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model;

public record class BlockFoldingInfo : NamedDocumentOffset
{
    public BlockFoldingInfo(string name, DocumentOffset offset, bool isDefinition = false)
        : base(name, offset)
    {
        IsDefinition = isDefinition;
    }

    public bool IsDefinition { get; }
}
