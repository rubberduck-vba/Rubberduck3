using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.UI.Abstract;
using System;

namespace Rubberduck.Core.Editor
{
    public class SyntaxErrorViewModel : ISyntaxErrorViewModel
    {
        #region TODO localize
        private static readonly string ExpectedIdentifierError = "An identifier name is expected.";
        private static readonly string ExpectedProcedureError = "A Sub, Function, or Property procedure declaration is expected.";
        private static readonly string ExpectedTokenError = "'{0}' is expected.";
        #endregion

        public SyntaxErrorViewModel(SyntaxError error)
        {
            StartOffset = error.OffendingToken.StartIndex;
            var endOffset = error.OffendingToken.StopIndex;
            Length = Math.Max(StartOffset, endOffset) - Math.Min(StartOffset, endOffset) + 1;

            switch (error.Context)
            {
                case VBAParser.IdentifierContext id when id.Parent.Parent is VBAParser.SubStmtContext parentSubStmt:
                    var parentSubToken = parentSubStmt.SUB();
                    StartOffset = parentSubToken.Symbol.StartIndex;
                    Length = parentSubToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;

                case VBAParser.IdentifierContext _:
                    Message = ExpectedIdentifierError;
                    break;

                case VBAParser.ModuleBodyElementContext _:
                    Message = ExpectedProcedureError;
                    break;

                case VBAParser.SubStmtContext subStmtMissingEndSub when subStmtMissingEndSub.END_SUB() is null:
                    var subTokenMissingEndSub = subStmtMissingEndSub.SUB();
                    StartOffset = subTokenMissingEndSub.Symbol.StartIndex;
                    Length = subTokenMissingEndSub.Symbol.StopIndex + 1 - StartOffset;
                    Message = string.Format(ExpectedTokenError, Tokens.EndSub);
                    break;
                case VBAParser.FunctionStmtContext functionStmtMissingEndFunction when functionStmtMissingEndFunction.END_FUNCTION() is null:
                    var functionTokenMissingEndSub = functionStmtMissingEndFunction.FUNCTION();
                    StartOffset = functionTokenMissingEndSub.Symbol.StartIndex;
                    Length = functionTokenMissingEndSub.Symbol.StopIndex + 1 - StartOffset;
                    Message = string.Format(ExpectedTokenError, Tokens.EndFunction);
                    break;
                case VBAParser.PropertyGetStmtContext propGetStmtMissingEndProperty when propGetStmtMissingEndProperty.END_PROPERTY() is null:
                    var propGetTokenMissingEndProperty = propGetStmtMissingEndProperty.PROPERTY_GET();
                    StartOffset = propGetTokenMissingEndProperty.Symbol.StartIndex;
                    Length = propGetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
                    Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
                    break;
                case VBAParser.PropertyLetStmtContext propLetStmtMissingEndProperty when propLetStmtMissingEndProperty.END_PROPERTY() is null:
                    var propLetTokenMissingEndProperty = propLetStmtMissingEndProperty.PROPERTY_LET();
                    StartOffset = propLetTokenMissingEndProperty.Symbol.StartIndex;
                    Length = propLetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
                    Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
                    break;
                case VBAParser.PropertySetStmtContext propSetStmtMissingEndProperty when propSetStmtMissingEndProperty.END_PROPERTY() is null:
                    var propSetTokenMissingEndProperty = propSetStmtMissingEndProperty.PROPERTY_SET();
                    StartOffset = propSetTokenMissingEndProperty.Symbol.StartIndex;
                    Length = propSetTokenMissingEndProperty.Symbol.StopIndex + 1 - StartOffset;
                    Message = string.Format(ExpectedTokenError, Tokens.EndProperty);
                    break;

                case VBAParser.SubStmtContext subStmt when subStmt.subroutineName() is null:
                    var subToken = subStmt.SUB();
                    StartOffset = subToken.Symbol.StartIndex;
                    Length = subToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;
                case VBAParser.FunctionStmtContext funcStmt when funcStmt.functionName() is null:
                    var functionToken = funcStmt.FUNCTION();
                    StartOffset = functionToken.Symbol.StartIndex;
                    Length = functionToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;
                case VBAParser.PropertyLetStmtContext propLetStmt when propLetStmt.subroutineName() is null:
                    var propertyLetToken = propLetStmt.PROPERTY_LET();
                    StartOffset = propertyLetToken.Symbol.StartIndex;
                    Length = propertyLetToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;
                case VBAParser.PropertySetStmtContext propSetStmt when propSetStmt.subroutineName() is null:
                    var propertySetToken = propSetStmt.PROPERTY_SET();
                    StartOffset = propertySetToken.Symbol.StartIndex;
                    Length = propertySetToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;
                case VBAParser.PropertyGetStmtContext propGetStmt when propGetStmt.functionName() is null:
                    var propertyGetToken = propGetStmt.PROPERTY_GET();
                    StartOffset = propertyGetToken.Symbol.StartIndex;
                    Length = propertyGetToken.Symbol.StopIndex + 1 - StartOffset + 1;
                    Message = ExpectedIdentifierError;
                    break;

                default:
                    break;
            }

            Line = error.OffendingToken.Line;
            Column = error.OffendingToken.Column + 1;
            ModuleName = error.ModuleName;
            Message = Message ?? error.Message;

            LocationMessage = $"Unexpected '{error.OffendingToken.Text}' token in {ModuleName} at offset {error.OffendingToken.StartIndex} (L{Line}C{Column})";
        }

        public int StartOffset { get; }
        public int Length { get; }
        public string Message { get; }
        public string LocationMessage { get; }
        public string ModuleName { get; }
        public int Line { get; }
        public int Column { get; }
    }
}