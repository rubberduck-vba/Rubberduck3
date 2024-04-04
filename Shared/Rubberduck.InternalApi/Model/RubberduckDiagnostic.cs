using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model;

public enum RubberduckDiagnosticId
{
    // TODO sort and categorize, then carve in stone.
    /*
     * [0]: Tokenization & parsing pass
     * [000]: IParseTree traversals
     * [0000]: Symbol traversals
     * [00000]: Execution pass
    */


    SyntaxError = 1,
    SllFailure = 3,

    ImplicitDeclarationsEnabled = 101, // [RD2:OptionExplicitInspection]
    ImplicitNonDefaultArrayBase, // [RD2: OptionBaseInspection]
    ImplicitTypeDeclarationsEnabled, // [Type]Def
    ImplicitByRefModifier,
    ImplicitPublicMember,
    ImplicitVariantDeclaration,
    ImplicitVariantReturnType,

    ImplicitStringCoercion,
    ImplicitNumericCoercion,
    ImplicitLetCoercion,
    ImplicitDateSerialConversion,
    ImplicitNarrowingConversion,
    ImplicitWideningConversion,

    IntegerDataTypeDeclaration = 201,
    ModuleScopeDimDeclaration,
    MultilineParameterDeclaration,
    MultipleDeclarations,
    //NotAllPathsReturnValue, // [RD2:NonReturningFunctionInspection]
    MisleadingByRefParameter, // property let/set value parameter is always passed ByVal

    ObsoleteCallingConvention = 301,
    ObsoleteCallStatement,
    ObsoleteCommentSyntax,
    //ObsoleteErrorSyntax,
    ObsoleteGlobalModifier,
    ObsoleteLetStatement,
    ObsoleteTypeHint,
    ObsoleteWhileWend,
    ObsoleteOnLocalErrorStatement,
    
    ObsoleteMemberUsage = 401, // members with an @Obsolete annotation
    InvalidAnnotation = 404, // @NotAnAnnotationButParsedLikeOne

    ImplementationsShouldBePrivate,
    PublicDeclarationInWorksheetModule, // [RD2:PublicEnumerationDeclaredInWorksheetInspection]

    // symbol traversals [0000]
    UseMeaningfulIdentifierNames = 1001,
    HungarianNotation,

    // execution pass diagnostics [00000]
    UnintendedConstantExpression = 11001,
    SuspiciousValueAssignment,
    TypeCastConversion,
    BitwiseOperator,
    PreferConcatOperatorForStringConcatenation,
    PreferErrRaiseOverErrorStatement,
}

public static class RubberduckDiagnosticIdExtensions
{
    public static string Code(this RubberduckDiagnosticId id) => $"RD3{(int)id:00000}";
    public static string Code(this VBCompileErrorId id) => $"VBC{(int)id:00000}";
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
            Range = symbol?.Range!,
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

    public static Diagnostic SllFailure(Symbol symbol) =>
        CreateDiagnostic(symbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.SllFailure, "SLL parser prediction mode failed here; if possible, rephrasing this instruction could improve parsing performance.");

    public static Diagnostic SllFailure(PredictionFailException error) =>
        CreateDiagnostic(error.OffendingSymbol, DiagnosticSeverity.Hint, RubberduckDiagnosticId.SllFailure, "SLL parser prediction mode failed here; if possible, rephrasing this instruction could improve parsing performance.");
    public static Diagnostic SyntaxError(SyntaxErrorException error) =>
        CreateDiagnostic(error.OffendingSymbol, DiagnosticSeverity.Error, RubberduckDiagnosticId.SyntaxError, error.Message, error.Uri.ToString());
}
