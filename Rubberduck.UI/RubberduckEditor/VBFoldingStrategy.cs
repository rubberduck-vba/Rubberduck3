using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.Abstract;

namespace Rubberduck.UI.RubberduckEditor
{
    internal class FoldStart : NewFolding
    {
        public int StartLine { get; set; }
        public string Ending { get; set; }
    }

    public class FoldingCodeBlockInfo
    {
        public FoldingCodeBlockInfo(string startToken, string endToken, MemberType memberType = MemberType.None, string startLineCompletion = "()")
        {
            StartToken = startToken;
            EndToken = endToken;
            StartLineCompletion = startLineCompletion;
            MemberType = memberType;
        }

        public MemberType MemberType { get; }
        public string StartLineCompletion { get; }
        public string StartToken { get; }
        public string EndToken { get; }
    }

    public class ScopeEventArgs : EventArgs
    {
        public ScopeEventArgs(int startOffset)
        {
            StartOffset = startOffset;
        }
        public ScopeEventArgs(string name, MemberType memberType, TextAnchor start, TextAnchor end)
        {
            Name = name;
            MemberType = memberType;
            StartAnchor = start;
            EndAnchor = end;
        }

        public int StartOffset { get; }

        public string Name { get; }
        public MemberType MemberType { get; }
        public TextAnchor StartAnchor { get; }
        public TextAnchor EndAnchor { get; }
    }

    public class VBFoldingStrategy : IFoldingStrategy
    {
        public static IDictionary<string,FoldingCodeBlockInfo> BlockInfo { get; } = new[] 
        { 
            new FoldingCodeBlockInfo("Sub", "End Sub", MemberType.Procedure),
            new FoldingCodeBlockInfo("Public Sub", "End Sub", MemberType.Procedure),
            new FoldingCodeBlockInfo("Private Sub", "End Sub", MemberType.ProcedurePrivate),
            new FoldingCodeBlockInfo("Friend Sub", "End Sub", MemberType.ProcedureFriend),
            new FoldingCodeBlockInfo("Function", "End Function", MemberType.Function),
            new FoldingCodeBlockInfo("Public Function", "End Function", MemberType.Function),
            new FoldingCodeBlockInfo("Private Function", "End Function", MemberType.FunctionPrivate),
            new FoldingCodeBlockInfo("Friend Function", "End Function", MemberType.FunctionFriend),
            new FoldingCodeBlockInfo("Property Get", "End Property", MemberType.PropertyGet),
            new FoldingCodeBlockInfo("Public Property Get", "End Property", MemberType.PropertyGet),
            new FoldingCodeBlockInfo("Private Property Get", "End Property", MemberType.PropertyGetPrivate),
            new FoldingCodeBlockInfo("Friend Property Get", "End Property", MemberType.PropertyGetFriend),
            new FoldingCodeBlockInfo("Property Let", "End Property", MemberType.PropertyLet),
            new FoldingCodeBlockInfo("Public Property Let", "End Property", MemberType.PropertyLet),
            new FoldingCodeBlockInfo("Private Property Let", "End Property", MemberType.PropertyLetPrivate),
            new FoldingCodeBlockInfo("Friend Property Let", "End Property", MemberType.PropertyLetFriend),
            new FoldingCodeBlockInfo("Property Set", "End Property", MemberType.PropertySet),
            new FoldingCodeBlockInfo("Public Property Set", "End Property", MemberType.PropertySet),
            new FoldingCodeBlockInfo("Private Property Set", "End Property", MemberType.PropertySetPrivate),
            new FoldingCodeBlockInfo("Friend Property Set", "End Property", MemberType.PropertySetFriend),
            new FoldingCodeBlockInfo("Type", "End Type", MemberType.UserDefinedType, startLineCompletion: null),
            new FoldingCodeBlockInfo("Public Type", "End Type", MemberType.UserDefinedType, startLineCompletion: null),
            new FoldingCodeBlockInfo("Private Type", "End Type", MemberType.UserDefinedTypePrivate, startLineCompletion: null),
            new FoldingCodeBlockInfo("Friend Type","End Type", MemberType.UserDefinedTypeFriend, startLineCompletion: null),
            new FoldingCodeBlockInfo("Enum", "End Enum", MemberType.Enum, startLineCompletion: null),
            new FoldingCodeBlockInfo("Public Enum", "End Enum", MemberType.Enum, startLineCompletion: null),
            new FoldingCodeBlockInfo("Private Enum", "End Enum", MemberType.EnumPrivate, startLineCompletion: null),
            new FoldingCodeBlockInfo("Friend Enum", "End Enum", MemberType.EnumFriend, startLineCompletion: null),
            new FoldingCodeBlockInfo("With", "End With", startLineCompletion: null),
            new FoldingCodeBlockInfo("If", "End If", startLineCompletion: " Then"),
            new FoldingCodeBlockInfo("For", "Next", startLineCompletion: null),
            new FoldingCodeBlockInfo("While", "Wend", startLineCompletion: null),
            new FoldingCodeBlockInfo("Do", "Loop", startLineCompletion: null),
            new FoldingCodeBlockInfo("Select Case", "End Select", startLineCompletion: null),

            new FoldingCodeBlockInfo("'@Region", "'@EndRegion", startLineCompletion: null),

        }.ToDictionary(e => e.StartToken);

        public event EventHandler<ScopeEventArgs> ScopeCreated;
        public event EventHandler<ScopeEventArgs> ScopeRemoved;

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(manager, document, out var firstErrorOffset).OrderBy(e => e.StartOffset);            
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(FoldingManager manager, TextDocument document, out int firstErrorOffset)
        {
            try
            {
                using (var reader = document.CreateReader())
                {
                    return CreateNewFoldings(manager, document, reader, out firstErrorOffset);
                }
            }
            catch
            {
                firstErrorOffset = 0;
                return Enumerable.Empty<NewFolding>();
            }
        }

        private IEnumerable<NewFolding> CreateNewFoldings(FoldingManager manager, TextDocument document, TextReader reader, out int firstErrorOffset)
        {
            var blocks = new Stack<FoldingCodeBlockInfo>();
            var stack = new Stack<(FoldStart Fold, string Name)>();
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
                            IsDefinition = blockInfo.MemberType != MemberType.None,
                        };

                        var name = Regex.Match(trimmed.Substring(blockInfo.StartToken.Length), @"\w+").Value;

                        stack.Push((start, name));
                        blocks.Push(blockInfo);
                    }
                    else if (stack.Any())
                    {
                        if (trimmed.StartsWith(stack.Peek().Fold.Ending))
                        {
                            var start = stack.Pop();
                            var block = blocks.Pop();
                            var offset = document.GetOffset(lineNumber, line.Length + 1);
                            var folding = new NewFolding(start.Fold.StartOffset, offset)
                            {
                                Name = start.Name,
                                IsDefinition = block.MemberType != MemberType.None,
                                DefaultClosed = false,
                            };
                            
                            TextAnchor startAnchor = null;
                            TextAnchor endAnchor = null;
                            if (block.MemberType != MemberType.None)
                            {
                                startAnchor = document.CreateAnchor(start.Fold.StartOffset);
                                endAnchor = document.CreateAnchor(offset);
                                ScopeCreated?.Invoke(this, new ScopeEventArgs(start.Name, block.MemberType, startAnchor, endAnchor));
                            }
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
