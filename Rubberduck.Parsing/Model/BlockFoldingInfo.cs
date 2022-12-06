namespace Rubberduck.Parsing.Model
{
    public class BlockFoldingInfo : NamedOffsetInfo
    {
        public BlockFoldingInfo(string name, DocumentOffset offset, bool isDefinition = false)
            : base(name, offset)
        {
            IsDefinition = isDefinition;
        }

        public bool IsDefinition { get; }
    }
}
