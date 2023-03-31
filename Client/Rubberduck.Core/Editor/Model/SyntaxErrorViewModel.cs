using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Editor
{
    public class SyntaxErrorViewModel : ISyntaxErrorViewModel
    {
        /* client-side cannot reference server-side libraries like this; server needs to supply the info...
         * we only need a serializable type accessible on the client side; the interface is probably superfluous then.
         */

        //#region TODO localize
        //private static readonly string ExpectedIdentifierError = "An identifier name is expected.";
        //private static readonly string ExpectedProcedureError = "A Sub, Function, or Property procedure declaration is expected.";
        //private static readonly string ExpectedTokenError = "'{0}' is expected.";
        //private static readonly string TypeCannotBeEmptyError = "User-defined type cannot be empty.";
        //#endregion

        //public SyntaxErrorViewModel(SyntaxErrorInfo error)
        //{
        //    StartOffset = error.OffendingSymbol.StartIndex;
        //    var endOffset = error.OffendingSymbol.StopIndex;
        //    Length = Math.Max(StartOffset, endOffset) - Math.Min(StartOffset, endOffset) + 1;

        //    switch (error.Exception.Context)
        //    {
        //        case VBAParser.UdtMemberListContext udtMemberList when !udtMemberList.udtMember()?.Any() ?? false:
        //            var parentTypeToken = ((VBAParser.UdtDeclarationContext)udtMemberList.Parent).TYPE();
        //            StartOffset = parentTypeToken.Symbol.StartIndex;
        //            Length = parentTypeToken.Symbol.StopIndex + 1 - StartOffset;
        //            Message = TypeCannotBeEmptyError;
        //            break;
        //        case VBAParser.IdentifierContext id when id.Parent.Parent is VBAParser.SubStmtContext parentSubStmt:
        //            var parentSubToken = parentSubStmt.SUB();
        //            StartOffset = parentSubToken.Symbol.StartIndex;
        //            Length = parentSubToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;

        //        case VBAParser.IdentifierContext _:
        //            Message = ExpectedIdentifierError;
        //            break;

        //        case VBAParser.ModuleBodyElementContext _:
        //            Message = ExpectedProcedureError;
        //            break;

        //        case VBAParser.SubStmtContext subStmtMissingEndSub when subStmtMissingEndSub.END_SUB() is null:
        //            var subTokenMissingEndSub = subStmtMissingEndSub.SUB();
        //            StartOffset = subTokenMissingEndSub.Symbol.StartIndex;
        //            Length = subTokenMissingEndSub.Symbol.StopIndex + 1 - StartOffset;
        //            Message = string.Format(ExpectedTokenError, Tokens.EndSub);
        //            break;
        //        case VBAParser.FunctionStmtContext functionStmtMissingEndFunction when functionStmtMissingEndFunction.END_FUNCTION() is null:
        //            var functionTokenMissingEndSub = functionStmtMissingEndFunction.FUNCTION();
        //            StartOffset = functionTokenMissingEndSub.Symbol.StartIndex;
        //            Length = functionTokenMissingEndSub.Symbol.StopIndex + 1 - StartOffset;
        //            Message = string.Format(ExpectedTokenError, Tokens.EndFunction);
        //            break;
        //        case VBAParser.PropertyGetStmtContext propGetStmtMissingEndProperty when propGetStmtMissingEndProperty.END_PROPERTY() is null:
        //            var propGetTokenMissingEndProperty = propGetStmtMissingEndProperty.PROPERTY_GET();
        //            StartOffset = propGetTokenMissingEndProperty.Symbol.StartIndex;
        //            Length = propGetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
        //            Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
        //            break;
        //        case VBAParser.PropertyLetStmtContext propLetStmtMissingEndProperty when propLetStmtMissingEndProperty.END_PROPERTY() is null:
        //            var propLetTokenMissingEndProperty = propLetStmtMissingEndProperty.PROPERTY_LET();
        //            StartOffset = propLetTokenMissingEndProperty.Symbol.StartIndex;
        //            Length = propLetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
        //            Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
        //            break;
        //        case VBAParser.PropertySetStmtContext propSetStmtMissingEndProperty when propSetStmtMissingEndProperty.END_PROPERTY() is null:
        //            var propSetTokenMissingEndProperty = propSetStmtMissingEndProperty.PROPERTY_SET();
        //            StartOffset = propSetTokenMissingEndProperty.Symbol.StartIndex;
        //            Length = propSetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
        //            Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
        //            break;

        //        case VBAParser.SubStmtContext subStmt when subStmt.subroutineName() is null:
        //            var subToken = subStmt.SUB();
        //            StartOffset = subToken.Symbol.StartIndex;
        //            Length = subToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;
        //        case VBAParser.FunctionStmtContext funcStmt when funcStmt.functionName() is null:
        //            var functionToken = funcStmt.FUNCTION();
        //            StartOffset = functionToken.Symbol.StartIndex;
        //            Length = functionToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;
        //        case VBAParser.PropertyLetStmtContext propLetStmt when propLetStmt.subroutineName() is null:
        //            var propertyLetToken = propLetStmt.PROPERTY_LET();
        //            StartOffset = propertyLetToken.Symbol.StartIndex;
        //            Length = propertyLetToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;
        //        case VBAParser.PropertySetStmtContext propSetStmt when propSetStmt.subroutineName() is null:
        //            var propertySetToken = propSetStmt.PROPERTY_SET();
        //            StartOffset = propertySetToken.Symbol.StartIndex;
        //            Length = propertySetToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;
        //        case VBAParser.PropertyGetStmtContext propGetStmt when propGetStmt.functionName() is null:
        //            var propertyGetToken = propGetStmt.PROPERTY_GET();
        //            StartOffset = propertyGetToken.Symbol.StartIndex;
        //            Length = propertyGetToken.Symbol.StopIndex + 1 - StartOffset + 1;
        //            Message = ExpectedIdentifierError;
        //            break;

        //        default:
        //            break;
        //    }

        //    Line = error.OffendingSymbol.Line;
        //    Column = error.OffendingSymbol.Column + 1;
        //    ModuleName = error.ModuleName;
        //    Message = Message ?? error.Message;

        //    LocationMessage = $"Unexpected '{error.OffendingSymbol.Text}' token in {ModuleName} at offset {error.OffendingSymbol.StartIndex} (L{Line}C{Column})";
        //}

        public string ModuleName { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int StartOffset { get; set; }
        public int Length { get; set; }

        public string Message { get; set; }
        public string LocationMessage { get; set; }
    }
}