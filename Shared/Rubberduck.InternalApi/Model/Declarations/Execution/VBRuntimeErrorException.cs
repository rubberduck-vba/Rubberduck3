using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public enum VBCompileErrorId
{
    ForbiddenWithOptionStrict = 9000,
    InvalidUseOfObject,
    ExpectedArray,
}

public interface IDiagnosticSource
{
    IEnumerable<Diagnostic> Diagnostics { get; }
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VBCompileErrorException : ApplicationException, IDiagnosticSource
{
    private string DebuggerDisplay => $"{Message}{(Verbose is null ? string.Empty : " | " + Verbose)}";

    #region Classic-VB compile-time errors
    // NOTE: VB compile errors are just messages, ID is made up.
    public static VBCompileErrorException OptionStrictForbidden(Symbol symbol, string? verbose = null) => new(symbol, VBCompileErrorId.ForbiddenWithOptionStrict, "Option Strict forbidden implicit narrowing conversion or late-bound call.", verbose);
    public static VBCompileErrorException InvalidUseOfObject(Symbol symbol, string? verbose = null) => new(symbol, VBCompileErrorId.InvalidUseOfObject, "Invalid use of object", verbose);
    public static VBCompileErrorException ExpectedArray(Symbol symbol, string? verbose = null) => new(symbol, VBCompileErrorId.ExpectedArray, "Expected array", verbose);
    #endregion

    public VBCompileErrorException(Symbol symbol, VBCompileErrorId id, string message, string? verbose = null)
        : base($"Compile error: {message}")
    {
        Id = (int)id;
        Symbol = symbol;
        Verbose = verbose;
    }

    public string DiagnosticCode => $"VBC{Id:00000}";
    public int Id { get; }
    public Symbol Symbol { get; }
    public string? Verbose { get; }

    public Diagnostic Diagnostic => RubberduckDiagnostic.CompileError(this);
    public IEnumerable<Diagnostic> Diagnostics => [Diagnostic];
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VBRuntimeErrorException : ApplicationException, IDiagnosticSource
{
    private string DebuggerDisplay => $"[{DiagnosticCode}] Error {VBErrorNumber}: {Message}{(Verbose is null ? string.Empty : " | " + Verbose)}";

    public static string GetErrorString(int errorNumber) => VBRuntimeErrors.TryGetValue(errorNumber, out var message) ? message : VBRuntimeErrors[-1];

    /// <summary>
    /// The Classic-VB runtime error numbers and messages - <strong>do not localize</strong>.
    /// </summary>
    private static readonly Dictionary<int, string> VBRuntimeErrors = new()
    {
        [-1] = "Application-defined or object-defined error",
        [3] = "Return without GoSub",
        [5] = "Invalid procedure call or argument",
        [6] = "Overflow",
        [7] = "Out of memory",
        [9] = "Subscript out of range",
        [10] = "This array is fixed or temporarily locked",
        [11] = "Division by zero",
        [13] = "Type mismatch",
        [14] = "Out of string space",
        [16] = "Expression too complex",
        [17] = "Can't perform requested operation",
        [18] = "User interrupt occurred",
        [20] = "Resume without error",
        [28] = "Out of stack space",
        [35] = "Sub or Function not defined",
        [47] = "Too many DLL application clients",
        [48] = "Error in loading DLL",
        [49] = "Bad DLL calling convention",
        [51] = "Internal error",
        [52] = "Bad file name or number",
        [53] = "File not found",
        [54] = "Bad file mode",
        [55] = "File already open",
        [57] = "Devise I/O error",
        [58] = "File already exists",
        [59] = "Bad record length",
        [61] = "Disk full",
        [62] = "Input past end of file",
        [63] = "Bad record number",
        [67] = "Too many files",
        [68] = "Device unavailable",
        [70] = "Permission denied",
        [71] = "Disk not ready",
        [74] = "Can't rename with different drive",
        [75] = "Path/File access error",
        [76] = "Path not found",
        [91] = "Object variable or With block variable not set",
        [92] = "For loop not initialized",
        [93] = "Invalid pattern string",
        [94] = "Invalid use of Null",
        [96] = "Unable to sink events of object because the object is already firing events to the maximum number of event receivers that it supports",
        [97] = "Can not call friend function on object which is not an instance of defining class",
        [98] = "A property or method call cannot include a reference to a private object, either as an argument or as a return value",
        [321] = "Invalid file format",
        [322] = "Can't create necessary temporary file",
        [325] = "Invalid format in resource file",
        [380] = "Invalid property value",
        [381] = "Invalid property array index",
        [382] = "Set not supported at runtime",
        [383] = "Set not supported (read-only property)",
        [385] = "Need property array index",
        [387] = "Set not permitted",
        [393] = "Get not supported at runtime",
        [394] = "Get not supported (write-only property)",
        [422] = "Property not found",
        [423] = "Property or method not found",
        [424] = "Object required",
        [429] = "ActiveX component can't create object",
        [430] = "Class does not support Automation or does not support expected interface",
        [432] = "File name or class name not found during Automation operation",
        [438] = "Object doesn't support this property or method",
        [440] = "Automation error",
        [442] = "Connection to type library or object library for remote process has been lost. Press OK for dialog to remove reference.",
        [443] = "Automation object does not have a default value",
        [445] = "Object doesn't support this action",
        [446] = "Object doesn't support named arguments",
        [447] = "Object doesn't support current locale setting",
        [448] = "Named argument not found",
        [449] = "Argument not optional",
        [450] = "Wrong number of arguments or invalid property assignment",
        [451] = "Property let procedure not defined and property get procedure did not return an object",
        [452] = "Invalid ordinal",
        [453] = "Specified DLL function not found",
        [454] = "Code resource not found",
        [455] = "Code resource lock error",
        [457] = "This key is already associated with an element of this collection",
        [458] = "Variable uses an Automation type not supported in Visual Basic",
        [459] = "Object or class does not support the set of events.",
        [460] = "Invalid clipboard format",
        [461] = "Method or data member not found",
        [462] = "The remote machine does not exist or is unavailable",
        [463] = "Class not registered on local machine",
        [481] = "Invalid picture",
        [482] = "Printer error",
        [735] = "Can't save file to TEMP",
        [744] = "Search text not found",
        [746] = "Replacements too long"
    };

    #region Classic-VB run-time errors
    public static VBRuntimeErrorException ReturnWithoutGoSub(Symbol symbol, string? verbose = null) => new(symbol, 3, verbose: verbose);
    public static VBRuntimeErrorException InvalidProcedureCallOrArgument(Symbol symbol, string? verbose = null) => new(symbol, 5, verbose: verbose);
    public static VBRuntimeErrorException Overflow(Symbol symbol, string? verbose = null) => new(symbol, 6, verbose: verbose);
    public static VBRuntimeErrorException OutOfMemory(Symbol symbol, string? verbose = null) => new(symbol, 7, verbose: verbose);
    public static VBRuntimeErrorException SubscriptOutOfRange(Symbol symbol, string? verbose = null) => new(symbol, 9, verbose: verbose);
    public static VBRuntimeErrorException ArrayIsFixedOrLocked(Symbol symbol, string? verbose = null) => new(symbol, 10, verbose: verbose);
    public static VBRuntimeErrorException DivisionByZero(Symbol symbol, string? verbose = null) => new(symbol, 11, verbose: verbose);
    public static VBRuntimeErrorException TypeMismatch(Symbol symbol, string? verbose = null) => new(symbol, 13, verbose: verbose);
    public static VBRuntimeErrorException OutOfStringSpace(Symbol symbol, string? verbose = null) => new(symbol, 14, verbose: verbose);
    public static VBRuntimeErrorException ExpressionTooComplex(Symbol symbol, string? verbose = null) => new(symbol, 16, verbose: verbose);
    public static VBRuntimeErrorException CantPerformRequestedOperation(Symbol symbol, string? verbose = null) => new(symbol, 17, verbose: verbose);
    public static VBRuntimeErrorException UserInterruptOccurred(Symbol symbol, string? verbose = null) => new(symbol, 18, verbose: verbose);
    public static VBRuntimeErrorException ResumeWithoutError(Symbol symbol, string? verbose = null) => new(symbol, 20, verbose: verbose);
    public static VBRuntimeErrorException OutOfStackSpace(Symbol symbol, string? verbose = null) => new(symbol, 28, verbose: verbose);
    public static VBRuntimeErrorException SubOrFunctionNotDefined(Symbol symbol, string? verbose = null) => new(symbol, 35, verbose: verbose);
    public static VBRuntimeErrorException TooManyDllApplicationClients(Symbol symbol, string? verbose = null) => new(symbol, 47, verbose: verbose);
    public static VBRuntimeErrorException ErrorLoadingDll(Symbol symbol, string? verbose = null) => new(symbol, 48, verbose: verbose);
    public static VBRuntimeErrorException BadDllCallingConvention(Symbol symbol, string? verbose = null) => new(symbol, 49, verbose: verbose);
    public static VBRuntimeErrorException InternalError(Symbol symbol, string? verbose = null) => new(symbol, 51, verbose: verbose);
    public static VBRuntimeErrorException BadFileNameOrNumber(Symbol symbol, string? verbose = null) => new(symbol, 52, verbose: verbose);
    public static VBRuntimeErrorException FileNorFound(Symbol symbol, string? verbose = null) => new(symbol, 53, verbose: verbose);
    public static VBRuntimeErrorException BadFileMode(Symbol symbol, string? verbose = null) => new(symbol, 54, verbose: verbose);
    public static VBRuntimeErrorException FileAlreadyOpen(Symbol symbol, string? verbose = null) => new(symbol, 55, verbose: verbose);
    public static VBRuntimeErrorException DeviseIOError(Symbol symbol, string? verbose = null) => new(symbol, 57, verbose: verbose);
    public static VBRuntimeErrorException FileAlreadyExists(Symbol symbol, string? verbose = null) => new(symbol, 58, verbose: verbose);
    public static VBRuntimeErrorException BadRecordLength(Symbol symbol, string? verbose = null) => new(symbol, 59, verbose: verbose);
    public static VBRuntimeErrorException DiskFull(Symbol symbol, string? verbose = null) => new(symbol, 61, verbose: verbose);
    public static VBRuntimeErrorException InputPastEndOfFile(Symbol symbol, string? verbose = null) => new(symbol, 62, verbose: verbose);
    public static VBRuntimeErrorException BadRecordNumber(Symbol symbol, string? verbose = null) => new(symbol, 63, verbose: verbose);
    public static VBRuntimeErrorException TooManyFiles(Symbol symbol, string? verbose = null) => new(symbol, 67, verbose: verbose);
    public static VBRuntimeErrorException DeviceUnavailable(Symbol symbol, string? verbose = null) => new(symbol, 68, verbose: verbose);
    public static VBRuntimeErrorException PermissionDenied(Symbol symbol, string? verbose = null) => new(symbol, 70, verbose: verbose);
    public static VBRuntimeErrorException DiskNotReady(Symbol symbol, string? verbose = null) => new(symbol, 71, verbose: verbose);
    public static VBRuntimeErrorException CantRenameWithDifferentDrive(Symbol symbol, string? verbose = null) => new(symbol, 74, verbose: verbose);
    public static VBRuntimeErrorException PathFileAccessError(Symbol symbol, string? verbose = null) => new(symbol, 75, verbose: verbose);
    public static VBRuntimeErrorException PathNotFound(Symbol symbol, string? verbose = null) => new(symbol, 76, verbose: verbose);
    public static VBRuntimeErrorException ObjectVariableNotSet(Symbol symbol, string? verbose = null) => new(symbol, 91, verbose: verbose);
    public static VBRuntimeErrorException ForLoopNotInitialized(Symbol symbol, string? verbose = null) => new(symbol, 92, verbose: verbose);
    public static VBRuntimeErrorException InvalidPatternString(Symbol symbol, string? verbose = null) => new(symbol, 93, verbose: verbose);
    public static VBRuntimeErrorException InvalidUseOfNull(Symbol symbol, string? verbose = null) => new(symbol, 94, verbose: verbose);
    public static VBRuntimeErrorException CannotSinkEvents(Symbol symbol, string? verbose = null) => new(symbol, 96, verbose: verbose);
    public static VBRuntimeErrorException CannotCallFriendFunction(Symbol symbol, string? verbose = null) => new(symbol, 97, verbose: verbose);
    public static VBRuntimeErrorException ReferenceToPrivateObject(Symbol symbol, string? verbose = null) => new(symbol, 98, verbose: verbose);
    public static VBRuntimeErrorException InvalidFileFormat(Symbol symbol, string? verbose = null) => new(symbol, 321, verbose: verbose);
    public static VBRuntimeErrorException CantCreateTempFile(Symbol symbol, string? verbose = null) => new(symbol, 322, verbose: verbose);
    public static VBRuntimeErrorException InvalidResourceFormat(Symbol symbol, string? verbose = null) => new(symbol, 325, verbose: verbose);
    public static VBRuntimeErrorException InvalidPropertyValue(Symbol symbol, string? verbose = null) => new(symbol, 380, verbose: verbose);
    public static VBRuntimeErrorException InvalidPropertyArrayIndex(Symbol symbol, string? verbose = null) => new(symbol, 381, verbose: verbose);
    public static VBRuntimeErrorException SetNotRuntimeSupported(Symbol symbol, string? verbose = null) => new(symbol, 382, verbose: verbose);
    public static VBRuntimeErrorException SetNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 383, verbose: verbose);
    public static VBRuntimeErrorException NeedPropertyArrayIndex(Symbol symbol, string? verbose = null) => new(symbol, 385, verbose: verbose);
    public static VBRuntimeErrorException SetNotPermitted(Symbol symbol, string? verbose = null) => new(symbol, 387, verbose: verbose);
    public static VBRuntimeErrorException GetNotRuntimeSupported(Symbol symbol, string? verbose = null) => new(symbol, 393, verbose: verbose);
    public static VBRuntimeErrorException GetNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 394, verbose: verbose);
    public static VBRuntimeErrorException PropertyNotFound(Symbol symbol, string? verbose = null) => new(symbol, 422, verbose: verbose);
    public static VBRuntimeErrorException PropertyOrMethodNotFound(Symbol symbol, string? verbose = null) => new(symbol, 423, verbose: verbose);
    public static VBRuntimeErrorException ObjectRequired(Symbol symbol, string? verbose = null) => new(symbol, 424, verbose: verbose);
    public static VBRuntimeErrorException ActiveXComponentCantCreateObject(Symbol symbol, string? verbose = null) => new(symbol, 429, verbose: verbose);
    public static VBRuntimeErrorException AutomationNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 430, verbose: verbose);
    public static VBRuntimeErrorException AutomationFileOrClassNameNotFound(Symbol symbol, string? verbose = null) => new(symbol, 432, verbose: verbose);
    public static VBRuntimeErrorException ObjectDoesntSupportPropertyOrMethod(Symbol symbol, string? verbose = null) => new(symbol, 438, verbose: verbose);
    public static VBRuntimeErrorException AutomationError(Symbol symbol, string? verbose = null) => new(symbol, 440, verbose: verbose);
    public static VBRuntimeErrorException RemoteProcessConnectionLost(Symbol symbol, string? verbose = null) => new(symbol, 442, verbose: verbose);
    public static VBRuntimeErrorException AutomationObjectHasNoDefaultValue(Symbol symbol, string? verbose = null) => new(symbol, 443, verbose: verbose);
    public static VBRuntimeErrorException UnsupportedObjectAction(Symbol symbol, string? verbose = null) => new(symbol, 445, verbose: verbose);
    public static VBRuntimeErrorException UnsupportedObjectNamedArguments(Symbol symbol, string? verbose = null) => new(symbol, 446, verbose: verbose);
    public static VBRuntimeErrorException UnsupportedObjectLocaleSetting(Symbol symbol, string? verbose = null) => new(symbol, 447, verbose: verbose);
    public static VBRuntimeErrorException NamedArgumentNotFound(Symbol symbol, string? verbose = null) => new(symbol, 448, verbose: verbose);
    public static VBRuntimeErrorException ArgumentNotOptional(Symbol symbol, string? verbose = null) => new(symbol, 449, verbose: verbose);
    public static VBRuntimeErrorException WrongNumberOfArgumentsOrInvalidPropertyAssignment(Symbol symbol, string? verbose = null) => new(symbol, 450, verbose: verbose);
    public static VBRuntimeErrorException PropertyLetNotDefinedPropertyGetNotAnObject(Symbol symbol, string? verbose = null) => new(symbol, 451, verbose: verbose);
    public static VBRuntimeErrorException InvalidOrdinal(Symbol symbol, string? verbose = null) => new(symbol, 452, verbose: verbose);
    public static VBRuntimeErrorException DllFunctionNotFound(Symbol symbol, string? verbose = null) => new(symbol, 453, verbose: verbose);
    public static VBRuntimeErrorException CodeResourceNotFound(Symbol symbol, string? verbose = null) => new(symbol, 454, verbose: verbose);
    public static VBRuntimeErrorException CodeResourceLockError(Symbol symbol, string? verbose = null) => new(symbol, 455, verbose: verbose);
    public static VBRuntimeErrorException KeyAlreadyExists(Symbol symbol, string? verbose = null) => new(symbol, 457, verbose: verbose);
    public static VBRuntimeErrorException UnsupportedAutomationType(Symbol symbol, string? verbose = null) => new(symbol, 458, verbose: verbose);
    public static VBRuntimeErrorException UnsupportedSetOfEvents(Symbol symbol, string? verbose = null) => new(symbol, 459, verbose: verbose);
    public static VBRuntimeErrorException InvalidClipboardFormat(Symbol symbol, string? verbose = null) => new(symbol, 460, verbose: verbose);
    public static VBRuntimeErrorException MethodOrDataMemberNotFound(Symbol symbol, string? verbose = null) => new(symbol, 461, verbose: verbose);
    public static VBRuntimeErrorException RemoteMachineUnavailable(Symbol symbol, string? verbose = null) => new(symbol, 462, verbose: verbose);
    public static VBRuntimeErrorException ClassNotRegistered(Symbol symbol, string? verbose = null) => new(symbol, 463, verbose: verbose);
    public static VBRuntimeErrorException InvalidPicture(Symbol symbol, string? verbose = null) => new(symbol, 481, verbose: verbose);
    public static VBRuntimeErrorException PrinterError(Symbol symbol, string? verbose = null) => new(symbol, 482, verbose: verbose);
    public static VBRuntimeErrorException CantSaveFileToTemp(Symbol symbol, string? verbose = null) => new(symbol, 735, verbose: verbose);
    public static VBRuntimeErrorException SearchTextNotFound(Symbol symbol, string? verbose = null) => new(symbol, 744, verbose: verbose);
    public static VBRuntimeErrorException ReplacementsTooLong(Symbol symbol, string? verbose = null) => new(symbol, 746, verbose: verbose);

    public static VBRuntimeErrorException ApplicationDefinedError(Symbol symbol, int number = 1004, string? verbose = null) => new (symbol, number, VBRuntimeErrors[-1], verbose);
    #endregion

    public VBRuntimeErrorException(Symbol symbol, int vBErrorNumber, string? message = null, string? verbose = null)
        : base($"Runtime error '{vBErrorNumber}': {message ?? (VBRuntimeErrors.TryGetValue(vBErrorNumber, out var errMessage) ? errMessage : VBRuntimeErrors[-1])}")
    {
        Symbol = symbol;
        VBErrorNumber = vBErrorNumber;
        Verbose = verbose;
    }

    public string DiagnosticCode => $"VBR{VBErrorNumber:00000}";
    public Symbol Symbol { get; }
    public int VBErrorNumber { get; }
    public string? Verbose { get; }

    public IEnumerable<Diagnostic> Diagnostics => [Diagnostic];
    public Diagnostic Diagnostic => RubberduckDiagnostic.RuntimeError(this);

    public (int, string) Deconstruct(out int vbErrorNumber, out string message) => 
        (vbErrorNumber = VBErrorNumber, message = Message);

    public (int, string, string?) Deconstruct(out int vbErrorNumber, out string message, out string? verbose) => 
        (vbErrorNumber = VBErrorNumber, message = Message, verbose = Verbose);
}
