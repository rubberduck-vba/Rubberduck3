using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;

namespace Rubberduck.InternalApi.Model;

public enum RubberduckDiagnosticId
{
    // TODO sort and categorize, then carve in stone.

    PreferConcatOperatorForStringConcatenation = 10,
    PreferErrRaiseOverErrorStatement,
    
    ImplicitStringCoercion = 101,
    ImplicitNumericCoercion,
    ImplicitLetCoercion,
    ImplicitDateSerialConversion,
    ImplicitNarrowingConversion,
    ImplicitWideningConversion,

    UnintendedConstantExpression = 1001,
    SuspiciousValueAssignment,
    TypeCastConversion,
    BitwiseOperator,
}

public record class RubberduckDiagnostic : Diagnostic
{
    private static Diagnostic CreateDiagnostic(Symbol symbol, DiagnosticSeverity severity, RubberduckDiagnosticId id, string message, string? source = null) =>
    CreateDiagnostic(symbol, severity, $"RD3{(int)id:00000}", message, source);
    private static Diagnostic CreateDiagnostic(Symbol symbol, DiagnosticSeverity severity, string code, string message, string? source = null) =>
        new()
        {
            Code = new DiagnosticCode(code),
            CodeDescription = new CodeDescription { Href = new Uri($"https://rd3.rubberduckvba.com/diagnostics/{code}") },
            Message = message,
            Severity = severity,
            Source = source ?? symbol.Uri.ToString(),
            Range = symbol.Range,
        };


    /* [VBC]: VB [C]ompile-time errors */
    public static Diagnostic CompileError(VBCompileErrorException error) =>
        CreateDiagnostic(error.Symbol, DiagnosticSeverity.Error, error.DiagnosticCode, error.Message, error.StackTrace);

    /* [VBR]: VB [R]un-time errors */
    public static Diagnostic RuntimeError(VBRuntimeErrorException error) => 
        CreateDiagnostic(error.Symbol, DiagnosticSeverity.Error, error.DiagnosticCode, error.Message, error.StackTrace);

    /* [RD3]: RD3 Language Server diagnostics */
    public static Diagnostic PreferConcatOperatorForStringConcatenation(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.PreferConcatOperatorForStringConcatenation, "Both operands are `String` values; consider using the `&` string concatenation operator instead.");
    public static Diagnostic PreferErrRaiseOverErrorStatement(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.PreferErrRaiseOverErrorStatement, "Consider using the `Err.Raise` method instead of the legacy `Error` statement to raise run-time errors.");
    public static Diagnostic ImplicitStringCoercion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitStringCoercion, "Implicit `String` coercion; consider using an explicit type conversion.");
    public static Diagnostic ImplicitNumericCoercion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitNumericCoercion, "Implicit numeric coercion; consider using an explicit type conversion.");
    public static Diagnostic ImplicitLetCoercion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitLetCoercion, "Implicit `Let` coercion; consider invoking the object's default member explicitly.");
    public static Diagnostic SuspiciousValueAssignment(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.SuspiciousValueAssignment, "Suspicious value assignment; since both LHS and RHS are object types, it looks like a reference assignment may have been intended. Are you missing a `Set` keyword?");
    public static Diagnostic ImplicitNarrowingConversion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitNarrowingConversion, "Implicit narrowing conversion; possible arithmetic overflow. Consider using a larger data type.");
    public static Diagnostic ImplicitWideningConversion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitWideningConversion, "Implicit widening conversion; consider using an explicit type conversion.");
    public static Diagnostic ImplicitDateSerialConversion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.ImplicitDateSerialConversion, "Implicit DateSerial conversion; consider using `VBA.DateTime` module functions to perform date and time operations.");
    public static Diagnostic TypeCastConversion(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.TypeCastConversion, "Assignment is converting RHS to a compatible interface type.");

    public static Diagnostic UnintendedConstantExpression(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.UnintendedConstantExpression, "Possibly unintended constant expression; this operation does not affect the value.");
    public static Diagnostic BitwiseOperator(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.BitwiseOperator, "Bitwise operator; the result of this operation is resolved using bitwise arithmetics.");
}
