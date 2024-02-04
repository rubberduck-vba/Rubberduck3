using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public enum VBCompileErrorId
{
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

    #region Classic-VB run-time errors
    public static VBRuntimeErrorException ReturnWithoutGoSub(Symbol symbol, string? verbose = null) => new(symbol, 3, "Return without GoSub", verbose);
    public static VBRuntimeErrorException InvalidProcedureCallOrArgument(Symbol symbol, string? verbose = null) => new(symbol, 5, "Invalid procedure call or argument", verbose);
    public static VBRuntimeErrorException Overflow(Symbol symbol, string? verbose = null) => new(symbol, 6, "Overflow", verbose);
    public static VBRuntimeErrorException OutOfMemory(Symbol symbol, string? verbose = null) => new(symbol, 7, "Out of memory", verbose);
    public static VBRuntimeErrorException SubscriptOutOfRange(Symbol symbol, string? verbose = null) => new(symbol, 9, "Subscript out of range", verbose);
    public static VBRuntimeErrorException ArrayIsFixedOrLocked(Symbol symbol, string? verbose = null) => new(symbol, 10, "This array is fixed or temporarily locked", verbose);
    public static VBRuntimeErrorException DivisionByZero(Symbol symbol, string? verbose = null) => new(symbol, 11, "Division by zero", verbose);
    public static VBRuntimeErrorException TypeMismatch(Symbol symbol, string? verbose = null) => new(symbol, 13, "Type mismatch", verbose);
    public static VBRuntimeErrorException OutOfStringSpace(Symbol symbol, string? verbose = null) => new(symbol, 14, "Out of string space", verbose);
    public static VBRuntimeErrorException ExpressionTooComplex(Symbol symbol, string? verbose = null) => new(symbol, 16, "Expression too complex", verbose);
    public static VBRuntimeErrorException CantPerformRequestedOperation(Symbol symbol, string? verbose = null) => new(symbol, 17, "Can't perform requested operation", verbose);
    public static VBRuntimeErrorException UserInterruptOccurred(Symbol symbol, string? verbose = null) => new(symbol, 18, "User interrupt occurred", verbose);
    public static VBRuntimeErrorException ResumeWithoutError(Symbol symbol, string? verbose = null) => new(symbol, 20, "Resume without error", verbose);
    public static VBRuntimeErrorException OutOfStackSpace(Symbol symbol, string? verbose = null) => new(symbol, 28, "Out of stack space", verbose);
    public static VBRuntimeErrorException SubOrFunctionNotDefined(Symbol symbol, string? verbose = null) => new(symbol, 35, "Sub or Function not defined", verbose);
    public static VBRuntimeErrorException TooManyDllApplicationClients(Symbol symbol, string? verbose = null) => new(symbol, 47, "Too many DLL application clients", verbose);
    public static VBRuntimeErrorException ErrorLoadingDll(Symbol symbol, string? verbose = null) => new(symbol, 48, "Error in loading DLL", verbose);
    public static VBRuntimeErrorException BadDllCallingConvention(Symbol symbol, string? verbose = null) => new(symbol, 49, "Bad DLL calling convention", verbose);
    public static VBRuntimeErrorException InternalError(Symbol symbol, string? verbose = null) => new(symbol, 51, "Internal error", verbose);
    public static VBRuntimeErrorException BadFileNameOrNumber(Symbol symbol, string? verbose = null) => new(symbol, 52, "Bad file name or number", verbose);
    public static VBRuntimeErrorException FileNorFound(Symbol symbol, string? verbose = null) => new(symbol, 53, "File not found", verbose);
    public static VBRuntimeErrorException BadFileMode(Symbol symbol, string? verbose = null) => new(symbol, 54, "Bad file mode", verbose);
    public static VBRuntimeErrorException FileAlreadyOpen(Symbol symbol, string? verbose = null) => new(symbol, 55, "File already open", verbose);
    public static VBRuntimeErrorException DeviseIOError(Symbol symbol, string? verbose = null) => new(symbol, 57, "Devise I/O error", verbose);
    public static VBRuntimeErrorException FileAlreadyExists(Symbol symbol, string? verbose = null) => new(symbol, 58, "File already exists", verbose);
    public static VBRuntimeErrorException BadRecordLength(Symbol symbol, string? verbose = null) => new(symbol, 59, "Bad record length", verbose);
    public static VBRuntimeErrorException DiskFull(Symbol symbol, string? verbose = null) => new(symbol, 61, "Disk full", verbose);
    public static VBRuntimeErrorException InputPastEndOfFile(Symbol symbol, string? verbose = null) => new(symbol, 62, "Input past end of file", verbose);
    public static VBRuntimeErrorException BadRecordNumber(Symbol symbol, string? verbose = null) => new(symbol, 63, "Bad record number", verbose);
    public static VBRuntimeErrorException TooManyFiles(Symbol symbol, string? verbose = null) => new(symbol, 67, "Too many files", verbose);
    public static VBRuntimeErrorException DeviceUnavailable(Symbol symbol, string? verbose = null) => new(symbol, 68, "Devise unavailable", verbose);
    public static VBRuntimeErrorException PermissionDenied(Symbol symbol, string? verbose = null) => new(symbol, 70, "Permission denied", verbose);
    public static VBRuntimeErrorException DiskNotReady(Symbol symbol, string? verbose = null) => new(symbol, 71, "Disk not ready", verbose);
    public static VBRuntimeErrorException CantRenameWithDifferentDrive(Symbol symbol, string? verbose = null) => new(symbol, 74, "Can't rename with different drive", verbose);
    public static VBRuntimeErrorException PathFileAccessError(Symbol symbol, string? verbose = null) => new(symbol, 75, "Path/File access error", verbose);
    public static VBRuntimeErrorException PathNotFound(Symbol symbol, string? verbose = null) => new(symbol, 76, "Path not found", verbose);
    public static VBRuntimeErrorException ObjectVariableNotSet(Symbol symbol, string? verbose = null) => new(symbol, 91, "Object variable or With block variable not set", verbose);
    public static VBRuntimeErrorException ForLoopNotInitialized(Symbol symbol, string? verbose = null) => new(symbol, 92, "For loop not initialized", verbose);
    public static VBRuntimeErrorException InvalidPatternString(Symbol symbol, string? verbose = null) => new(symbol, 93, "Invalid pattern string", verbose);
    public static VBRuntimeErrorException InvalidUseOfNull(Symbol symbol, string? verbose = null) => new(symbol, 94, "Invalid use of Null", verbose);
    public static VBRuntimeErrorException CannotSinkEvents(Symbol symbol, string? verbose = null) => new(symbol, 96, "Unable to sink events of object because the object is already firing events to the maximum number of event receivers that it supports", verbose);
    public static VBRuntimeErrorException CannotCallFriendFunction(Symbol symbol, string? verbose = null) => new(symbol, 97, "Can not call friend function on object which is not an instance of defining class", verbose);
    public static VBRuntimeErrorException ReferenceToPrivateObject(Symbol symbol, string? verbose = null) => new(symbol, 98, "A property or method call cannot include a reference to a private object, either as an argument or as a return value", verbose);
    public static VBRuntimeErrorException InvalidFileFormat(Symbol symbol, string? verbose = null) => new(symbol, 321, "Invalid file format", verbose);
    public static VBRuntimeErrorException CantCreateTempFile(Symbol symbol, string? verbose = null) => new(symbol, 322, "Can't create necessary temporary file", verbose);
    public static VBRuntimeErrorException InvalidResourceFormat(Symbol symbol, string? verbose = null) => new(symbol, 325, "Invalid format in resource file", verbose);
    public static VBRuntimeErrorException InvalidPropertyValue(Symbol symbol, string? verbose = null) => new(symbol, 380, "Invalid property value", verbose);
    public static VBRuntimeErrorException InvalidPropertyArrayIndex(Symbol symbol, string? verbose = null) => new(symbol, 381, "Invalid property array index", verbose);
    public static VBRuntimeErrorException SetNotRuntimeSupported(Symbol symbol, string? verbose = null) => new(symbol, 382, "Set not supported at runtime", verbose);
    public static VBRuntimeErrorException SetNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 383, "Set not supported (read-only property)", verbose);
    public static VBRuntimeErrorException NeedPropertyArrayIndex(Symbol symbol, string? verbose = null) => new(symbol, 385, "Need property array index", verbose);
    public static VBRuntimeErrorException SetNotPermitted(Symbol symbol, string? verbose = null) => new(symbol, 387, "Set not permitted", verbose);
    public static VBRuntimeErrorException GetNotRuntimeSupported(Symbol symbol, string? verbose = null) => new(symbol, 393, "Get not supported at runtime", verbose);
    public static VBRuntimeErrorException GetNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 394, "Get not supported (write-only property)", verbose);
    public static VBRuntimeErrorException PropertyNotFound(Symbol symbol, string? verbose = null) => new(symbol, 422, "Property not found", verbose);
    public static VBRuntimeErrorException PropertyOrMethodNotFound(Symbol symbol, string? verbose = null) => new(symbol, 423, "Property or method not found", verbose);
    public static VBRuntimeErrorException ObjectRequired(Symbol symbol, string? verbose = null) => new(symbol, 424, "Object required", verbose);
    public static VBRuntimeErrorException ActiveXComponentCantCreateObject(Symbol symbol, string? verbose = null) => new(symbol, 429, "ActiveX component can't create object", verbose);
    public static VBRuntimeErrorException AutomationNotSupported(Symbol symbol, string? verbose = null) => new(symbol, 430, "Class does not support Automation or does not support expected interface", verbose);
    public static VBRuntimeErrorException AutomationFileOrClassNameNotFound(Symbol symbol, string? verbose = null) => new(symbol, 432, "File name or class name not found during Automation operation", verbose);
    public static VBRuntimeErrorException ObjectDoesntSupportPropertyOrMethod(Symbol symbol, string? verbose = null) => new(symbol, 438, "Object doesn't support this property or method", verbose);
    public static VBRuntimeErrorException AutomationError(Symbol symbol, string? verbose = null) => new(symbol, 440, "Automation error", verbose);
    public static VBRuntimeErrorException RemoteProcessConnectionLost(Symbol symbol, string? verbose = null) => new(symbol, 442, "Connection to type library or object library for remote process has been lost. Press OK for dialog to remove reference.", verbose);
    public static VBRuntimeErrorException AutomationObjectHasNoDefaultValue(Symbol symbol, string? verbose = null) => new(symbol, 443, "Automation object does not have a default value", verbose);
    public static VBRuntimeErrorException UnsupportedObjectAction(Symbol symbol, string? verbose = null) => new(symbol, 445, "Object doesn't support this action", verbose);
    public static VBRuntimeErrorException UnsupportedObjectNamedArguments(Symbol symbol, string? verbose = null) => new(symbol, 446, "Object doesn't support named arguments", verbose);
    public static VBRuntimeErrorException UnsupportedObjectLocaleSetting(Symbol symbol, string? verbose = null) => new(symbol, 447, "Object doesn't support current locale setting", verbose);
    public static VBRuntimeErrorException NamedArgumentNotFound(Symbol symbol, string? verbose = null) => new(symbol, 448, "Named argument not found", verbose);
    public static VBRuntimeErrorException ArgumentNotOptional(Symbol symbol, string? verbose = null) => new(symbol, 449, "Argument not optional", verbose);
    public static VBRuntimeErrorException WrongNumberOfArgumentsOrInvalidPropertyAssignment(Symbol symbol, string? verbose = null) => new(symbol, 450, "Wrong number of arguments or invalid property assignment", verbose);
    public static VBRuntimeErrorException PropertyLetNotDefinedPropertyGetNotAnObject(Symbol symbol, string? verbose = null) => new(symbol, 451, "Property let procedure not defined and property get procedure did not return an object", verbose);
    public static VBRuntimeErrorException InvalidOrdinal(Symbol symbol, string? verbose = null) => new(symbol, 452, "Invalid ordinal", verbose);
    public static VBRuntimeErrorException DllFunctionNotFound(Symbol symbol, string? verbose = null) => new(symbol, 453, "Specified DLL function not found", verbose);
    public static VBRuntimeErrorException CodeResourceNotFound(Symbol symbol, string? verbose = null) => new(symbol, 454, "Code resource not found", verbose);
    public static VBRuntimeErrorException CodeResourceLockError(Symbol symbol, string? verbose = null) => new(symbol, 455, "Code resource lock error", verbose);
    public static VBRuntimeErrorException KeyAlreadyExists(Symbol symbol, string? verbose = null) => new(symbol, 457, "This key is already associated with an element of this collection", verbose);
    public static VBRuntimeErrorException UnsupportedAutomationType(Symbol symbol, string? verbose = null) => new(symbol, 458, "Variable uses an Automation type not supported in Visual Basic", verbose);
    public static VBRuntimeErrorException UnsupportedSetOfEvents(Symbol symbol, string? verbose = null) => new(symbol, 459, "Object or class does not support the set of events.", verbose);
    public static VBRuntimeErrorException InvalidClipboardFormat(Symbol symbol, string? verbose = null) => new(symbol, 460, "Invalid clipboard format", verbose);
    public static VBRuntimeErrorException MethodOrDataMemberNotFound(Symbol symbol, string? verbose = null) => new(symbol, 461, "Method or data member not found", verbose);
    public static VBRuntimeErrorException RemoteMachineUnavailable(Symbol symbol, string? verbose = null) => new(symbol, 462, "The remote machine does not exist or is unavailable", verbose);
    public static VBRuntimeErrorException ClassNotRegistered(Symbol symbol, string? verbose = null) => new(symbol, 463, "Class not registered on local machine", verbose);
    public static VBRuntimeErrorException InvalidPicture(Symbol symbol, string? verbose = null) => new(symbol, 481, "Invalid picture", verbose);
    public static VBRuntimeErrorException PrinterError(Symbol symbol, string? verbose = null) => new(symbol, 482, "Printer error", verbose);
    public static VBRuntimeErrorException CantSaveFileToTemp(Symbol symbol, string? verbose = null) => new(symbol, 735, "Can't save file to TEMP", verbose);
    public static VBRuntimeErrorException SearchTextNotFound(Symbol symbol, string? verbose = null) => new(symbol, 744, "Search text not found", verbose);
    public static VBRuntimeErrorException ReplacementsTooLong(Symbol symbol, string? verbose = null) => new(symbol, 746, "Replacements too long", verbose);

    public static VBRuntimeErrorException ApplicationDefinedError(Symbol symbol, int number = 1004, string? verbose = null) => new (symbol, number, "Application-defined or object-defined error", verbose);
    #endregion

    public VBRuntimeErrorException(Symbol symbol, int vBErrorNumber, string message, string? verbose = null)
        : base($"Runtime error '{vBErrorNumber}': {message}")
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
