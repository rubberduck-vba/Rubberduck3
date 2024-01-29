using Rubberduck.Unmanaged.VBERuntime;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public class VBRuntimeErrorException : ApplicationException
{
    public static VBRuntimeErrorException ReturnWithoutGoSub => new(3, "Return without GoSub");
    public static VBRuntimeErrorException InvalidProcedureCallOrArgument => new(5, "Invalid procedure call or argument");
    public static VBRuntimeErrorException Overflow => new(6, "Overflow");
    public static VBRuntimeErrorException OutOfMemory => new(7, "Out of memory");
    public static VBRuntimeErrorException SubscriptOutOfRange => new(9, "Subscript out of range");
    public static VBRuntimeErrorException ArrayIsFixedOrLocked => new(10, "This array is fixed or temporarily locked");
    public static VBRuntimeErrorException DivisionByZero => new(11, "Division by zero");
    public static VBRuntimeErrorException TypeMismatch => new(13, "Type mismatch");
    public static VBRuntimeErrorException OutOfStringSpace => new(14, "Out of string space");
    public static VBRuntimeErrorException ExpressionTooComplex => new(16, "Expression too complex");
    public static VBRuntimeErrorException CantPerformRequestedOperation => new(17, "Can't perform requested operation");
    public static VBRuntimeErrorException UserInterruptOccurred => new(18, "User interrupt occurred");
    public static VBRuntimeErrorException ResumeWithoutError => new(20, "Resume without error");
    public static VBRuntimeErrorException OutOfStackSpace => new(28, "Out of stack space");
    public static VBRuntimeErrorException SubOrFunctionNotDefined => new(35, "Sub or Function not defined");
    public static VBRuntimeErrorException TooManyDllApplicationClients => new(47, "Too many DLL application clients");
    public static VBRuntimeErrorException ErrorLoadingDll => new(48, "Error in loading DLL");
    public static VBRuntimeErrorException BadDllCallingConvention => new(49, "Bad DLL calling convention");
    public static VBRuntimeErrorException InternalError => new(51, "Internal error");
    public static VBRuntimeErrorException BadFileNameOrNumber => new(52, "Bad file name or number");
    public static VBRuntimeErrorException FileNorFound => new(53, "File not found");
    public static VBRuntimeErrorException BadFileMode => new(54, "Bad file mode");
    public static VBRuntimeErrorException FileAlreadyOpen => new(55, "File already open");
    public static VBRuntimeErrorException DeviseIOError => new(57, "Devise I/O error");
    public static VBRuntimeErrorException FileAlreadyExists => new(58, "File already exists");
    public static VBRuntimeErrorException BadRecordLength => new(59, "Bad record length");
    public static VBRuntimeErrorException DiskFull => new(61, "Disk full");
    public static VBRuntimeErrorException InputPastEndOfFile => new(62, "Input past end of file");
    public static VBRuntimeErrorException BadRecordNumber => new(63, "Bad record number");
    public static VBRuntimeErrorException TooManyFiles => new(67, "Too many files");
    public static VBRuntimeErrorException DeviceUnavailable => new(68, "Devise unavailable");
    public static VBRuntimeErrorException PermissionDenied => new(70, "Permission denied");
    public static VBRuntimeErrorException DiskNotReady => new(71, "Disk not ready");
    public static VBRuntimeErrorException CantRenameWithDifferentDrive => new(74, "Can't rename with different drive");
    public static VBRuntimeErrorException PathFileAccessError => new(75, "Path/File access error");
    public static VBRuntimeErrorException PathNotFound => new(76, "Path not found");
    public static VBRuntimeErrorException ObjectVariableNotSet => new(91, "Object variable or With block variable not set");
    public static VBRuntimeErrorException ForLoopNotInitialized => new(92, "For loop not initialized");
    public static VBRuntimeErrorException InvalidPatternString => new(93, "Invalid pattern string");
    public static VBRuntimeErrorException InvalidUseOfNull => new(94, "Invalid use of Null");
    public static VBRuntimeErrorException CannotSinkEvents => new(96, "Unable to sink events of object because the object is already firing events to the maximum number of event receivers that it supports");
    public static VBRuntimeErrorException CannotCallFriendFunction => new(97, "Can not call friend function on object which is not an instance of defining class");
    public static VBRuntimeErrorException ReferenceToPrivateObject => new(98, "A property or method call cannot include a reference to a private object, either as an argument or as a return value");
    public static VBRuntimeErrorException ApplicationDefinedError(int number) => new(number, "Application-defined or object-defined error");

    public VBRuntimeErrorException(int vBErrorNumber, string message, string? verbose = null)
        : base($"Runtime error '{vBErrorNumber}': {message}")
    {
        VBErrorNumber = vBErrorNumber;
        Verbose = verbose;
    }

    public int VBErrorNumber { get; }
    public string? Verbose { get; }
}
