using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rubberduck.UI.RubberduckEditor
{
    public class BlockCompletionService
    {
        private readonly IEnumerable<IBlockCompletionStrategy> _strategies;

        public BlockCompletionService(IEnumerable<IBlockCompletionStrategy> strategies)
        {
            _strategies = strategies ?? Enumerable.Empty<IBlockCompletionStrategy>();
        }

        public bool CanComplete(Caret caret, TextDocument document, IMemberInfoViewModel memberInfo, out IBlockCompletionStrategy completionStrategy, out string text)
        {
            completionStrategy = null;

            if (memberInfo is null)
            {
                text = null;
                return false;
            }

            var line = document.Lines[caret.Line - 1];
            if (memberInfo.HasImplementation && memberInfo.StartLine == caret.Line)
            {
                completionStrategy = null;
                text = null;
                return false;
            }

            text = document.GetText(line.Offset, line.Length);

            var match = Regex.Match(text, @"\w+");
            if (!match.Success || string.IsNullOrWhiteSpace(match.Value.Substring(memberInfo.MemberType.GetType().GetCustomAttribute<DisplayAttribute>()?.Name.Length ?? 0)))
            {
                return false;
            }

            var lineText = text;
            completionStrategy = _strategies.FirstOrDefault(e => e.CanComplete(lineText));

            return completionStrategy != null;
        }
    }
}
