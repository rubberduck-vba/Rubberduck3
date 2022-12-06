using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Model
{
    public readonly struct DocumentOffset
    {
        public DocumentOffset(VBABaseParserRuleContext context) 
            : this(context.Offset.Start, context.Offset.End) { }
        public DocumentOffset(int start) : this(start, start) { }

        public DocumentOffset(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public int Start { get; }
        public int End { get; }
        public int Length { get; }
    }
}
