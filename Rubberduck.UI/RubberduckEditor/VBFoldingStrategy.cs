using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using NLog.LayoutRenderers;

namespace Rubberduck.UI.RubberduckEditor
{
    internal class FoldStart : NewFolding
    {
        public int StartLine { get; set; }
        public string Ending { get; set; }
    }

    public class VBFoldingStrategy : IFoldingStrategy
    {
        private static readonly string[] _keywords = new[] 
        { 
            "Sub",
            "Public Sub",
            "Private Sub",
            "Friend Sub",
            "Function",
            "Public Function",
            "Private Function",
            "Friend Function",
            "Property",
            "Public Property",
            "Private Property",
            "Friend Property",
            "Type",
            "Public Type",
            "Private Type",
            "Friend Type",
            "Enum",
            "Public Enum",
            "Private Enum",
            "Friend Enum",
        };

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            try
            {
                using (var reader = document.CreateReader())
                {
                    return CreateNewFoldings(document, reader, out firstErrorOffset);
                }
            }
            catch
            {
                firstErrorOffset = 0;
                return Enumerable.Empty<NewFolding>();
            }
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, TextReader reader, out int firstErrorOffset)
        {
            var stack = new Stack<FoldStart>();
            var markers = new List<NewFolding>();

            var line = reader.ReadLine();
            var lineNumber = 1;
            while (line != null)
            {
                var trimmed = line.TrimStart();
                var keyword = _keywords.SingleOrDefault(k => trimmed.StartsWith(k + " ", StringComparison.InvariantCultureIgnoreCase));
                if (keyword != null)
                {
                    var ending = "End " + keyword.Split(' ').Last();
                    var offset = document.GetOffset(lineNumber, 0);

                    var start = new FoldStart
                    {
                        StartLine = lineNumber,
                        StartOffset = offset,
                        Ending = ending,
                        Name = trimmed,
                    };
                    stack.Push(start);
                }
                else if(stack.Any())
                {
                    if (trimmed.StartsWith(stack.Peek().Ending))
                    {
                        var start = stack.Pop();
                        var offset = document.GetOffset(lineNumber, line.Length + 1);
                        var folding = new NewFolding(start.StartOffset, offset)
                        {
                            Name = start.Name,
                            IsDefinition = true,
                            DefaultClosed = false,
                        };
                        markers.Add(folding);
                    }
                }

                line = reader.ReadLine();
                lineNumber++;
            }

            firstErrorOffset = 0;
            return markers;
        }
    }
}
