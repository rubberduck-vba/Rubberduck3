using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.UI.Xaml.Controls
{
    public class KeepCurrentLineTogetherEnterKeyStrategy : IEnterKeyStrategy
    {
        public bool IsActive { get; set; } = true; // TODO make it configurable

        private static readonly HashSet<MemberType> ApplicableMemberTypes = new HashSet<MemberType>(new []
        {
            MemberType.Procedure,
            MemberType.ProcedurePrivate,
            MemberType.ProcedureFriend,
            MemberType.Function,
            MemberType.FunctionPrivate,
            MemberType.FunctionFriend,
            MemberType.PropertyGet,
            MemberType.PropertyGetPrivate,
            MemberType.PropertyGetFriend,
            MemberType.PropertyLet,
            MemberType.PropertyLetPrivate,
            MemberType.PropertyLetFriend,
            MemberType.PropertySet,
            MemberType.PropertySetPrivate,
            MemberType.PropertySetFriend,
        });

        public bool HandleEvent(TextDocument document, ref int caretOffset)
        {
            var offset = caretOffset;
            var line = document.GetText(document.GetLineByOffset(offset));
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }
            var type = MemberEditor.GetMemberType(line);

            var nextChar = document.GetCharAt(offset); 
            if (nextChar == '(' && ApplicableMemberTypes.Contains(type))
            {
                var currentLine = document.GetLineByOffset(offset);
                var nextLineOffset = currentLine.NextLine.Offset; // TODO account for indentation
                caretOffset = nextLineOffset;
                return true;
            }

            return false;
        }
    }
}
