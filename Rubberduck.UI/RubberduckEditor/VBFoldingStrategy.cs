using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Rubberduck.UI.RubberduckEditor
{
    internal class FoldStart : NewFolding
    {
        public int StartLine { get; set; }
        public string Ending { get; set; }
    }

    public class CodeBlockInfo
    {
        public CodeBlockInfo(string startToken, string endToken, string startLineCompletion = null)
        {
            StartToken = startToken;
            EndToken = endToken;
            StartLineCompletion = startLineCompletion;
        }

        public string StartLineCompletion { get; }
        public string StartToken { get; }
        public string EndToken { get; }
    }

    public class VBFoldingStrategy : IFoldingStrategy
    {
        public static IDictionary<string,CodeBlockInfo> BlockInfo { get; } = new[] 
        { 
            new CodeBlockInfo("Sub", "End Sub", "()"),
            new CodeBlockInfo("Public Sub", "End Sub", "()"),
            new CodeBlockInfo("Private Sub", "End Sub", "()"),
            new CodeBlockInfo("Friend Sub", "End Sub", "()"),
            new CodeBlockInfo("Function", "End Function", "()"),
            new CodeBlockInfo("Public Function", "End Function", "()"),
            new CodeBlockInfo("Private Function", "End Function", "()"),
            new CodeBlockInfo("Friend Function", "End Function", "()"),
            new CodeBlockInfo("Property", "End Property", "()"),
            new CodeBlockInfo("Public Property", "End Property", "()"),
            new CodeBlockInfo("Private Property", "End Property", "()"),
            new CodeBlockInfo("Friend Property", "End Property", "()"),
            new CodeBlockInfo("Type", "End Type"),
            new CodeBlockInfo("Public Type", "End Type"),
            new CodeBlockInfo("Private Type", "End Type"),
            new CodeBlockInfo("Friend Type", "End Type"),
            new CodeBlockInfo("Enum", "End Enum"),
            new CodeBlockInfo("Public Enum", "End Enum"),
            new CodeBlockInfo("Private Enum", "End Enum"),
            new CodeBlockInfo("Friend Enum", "End Enum"),
            new CodeBlockInfo("With", "End With"),
            new CodeBlockInfo("If", "End If", "Then"),
            new CodeBlockInfo("For", "Next"),
            new CodeBlockInfo("While", "Wend"),
            new CodeBlockInfo("Do", "Loop"),
            new CodeBlockInfo("Select Case", "End Select")
        }.ToDictionary(e => e.StartToken);

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(document, out var firstErrorOffset).OrderBy(e => e.StartOffset);
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
            while (line != null && lineNumber < document.LineCount)
            {
                var trimmed = line.TrimStart();
                if (trimmed.Length > 0)
                {
                    var blockInfo = BlockInfo.Values.SingleOrDefault(k => trimmed.StartsWith(k.StartToken + " ", StringComparison.InvariantCultureIgnoreCase));
                    if (blockInfo != null)
                    {
                        var ending = blockInfo.EndToken;
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
                    else if (stack.Any())
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
                }
                line = reader.ReadLine();
                lineNumber++;
            }

            firstErrorOffset = 0;
            return markers;
        }
    }
}
