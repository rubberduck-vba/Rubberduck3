using Antlr4.Runtime;
//using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Grammar;

public abstract class VBABaseParserRuleContext : ParserRuleContext
{
    public VBABaseParserRuleContext() : base() { }
    
    public VBABaseParserRuleContext(ParserRuleContext parent, int invokingStateNumber) 
        : base(parent, invokingStateNumber) { }

    private DocumentOffset _offset = DocumentOffset.Invalid;

    public DocumentOffset Offset
    {
        get
        {
            //var anchorOffset = DocumentAnchor?.Offset;
            //if (!anchorOffset.HasValue || !_offset.Equals(anchorOffset.Value))
            {
                var startOffset = Start?.StartIndex ?? DocumentOffset.Invalid.Start;
                var endOffset = Stop?.StopIndex ?? DocumentOffset.Invalid.End;

                //_offset = new DocumentOffset(anchorOffset ?? startOffset, (anchorOffset ?? 0) - startOffset + endOffset);
            }

            return _offset;
        }
    }

    //public TextAnchor DocumentAnchor { get; private set; }

    //public void Anchor(TextDocument document)
    //{
    //    DocumentAnchor = document.CreateAnchor(Start.StartIndex);
    //}
}
