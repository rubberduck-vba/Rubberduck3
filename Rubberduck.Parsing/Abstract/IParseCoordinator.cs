namespace Rubberduck.Parsing.VBA.Parsing
{
    public interface IParseCoordinator
    {
        IParserStateManager ParserState { get; }
    }
}
