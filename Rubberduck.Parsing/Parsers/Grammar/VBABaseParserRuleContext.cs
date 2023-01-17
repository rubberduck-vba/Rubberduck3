using Antlr4.Runtime;
//using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Grammar
{
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

        public Selection Selection
        {
            get
            {
                // if we have an empty module, `Stop` is null
                if (Stop is null) { return Selection.Empty; }

                // ANTLR indexes for columns are 0-based, but VBE's are 1-based.
                // ANTLR lines and VBE's lines are both 1-based
                // 1 is the default value that will select all lines. Replace zeroes with ones.
                // See also: https://msdn.microsoft.com/en-us/library/aa443952(v=vs.60).aspx

                return new Selection(Start.Line == 0 ? 1 : Start.Line,
                                     Start.Column + 1,
                                     Stop.Line == 0 ? 1 : Stop.EndLine(),
                                     Stop.EndColumn() + 1);
            }
        }
    }
}
