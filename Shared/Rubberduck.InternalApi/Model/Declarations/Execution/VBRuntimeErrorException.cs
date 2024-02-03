using System;
using System.Diagnostics;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class VBRuntimeErrorException : ApplicationException
{
    private string DebuggerDisplay => $"Error {VBErrorNumber}: {Message}{(Verbose is null ? string.Empty : " | " + Verbose)}";

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
    public static VBRuntimeErrorException InvalidFileFormat => new(321, "Invalid file format");
    public static VBRuntimeErrorException CantCreateTempFile => new(322, "Can't create necessary temporary file");
    public static VBRuntimeErrorException InvalidResourceFormat => new(325, "Invalid format in resource file");
    public static VBRuntimeErrorException InvalidPropertyValue => new(380, "Invalid property value");
    public static VBRuntimeErrorException InvalidPropertyArrayIndex => new(381, "Invalid property array index");
    public static VBRuntimeErrorException SetNotRuntimeSupported => new(382, "Set not supported at runtime");
    public static VBRuntimeErrorException SetNotSupported => new(383, "Set not supported (read-only property)");
    public static VBRuntimeErrorException NeedPropertyArrayIndex => new(385, "Need property array index");
    public static VBRuntimeErrorException SetNotPermitted => new(387, "Set not permitted");
    public static VBRuntimeErrorException GetNotRuntimeSupported => new(393, "Get not supported at runtime");
    public static VBRuntimeErrorException GetNotSupported => new(394, "Get not supported (write-only property)");
    public static VBRuntimeErrorException PropertyNotFound => new(422, "Property not found");
    public static VBRuntimeErrorException PropertyOrMethodNotFound => new(423, "Property or method not found");
    public static VBRuntimeErrorException ObjectRequired => new(424, "Object required");
    public static VBRuntimeErrorException ActiveXComponentCantCreateObject => new(429, "ActiveX component can't create object");
    public static VBRuntimeErrorException AutomationNotSupported => new(430, "Class does not support Automation or does not support expected interface");
    public static VBRuntimeErrorException AutomationFileOrClassNameNotFound => new(432, "File name or class name not found during Automation operation");
    public static VBRuntimeErrorException ObjectDoesntSupportPropertyOrMethod => new(438, "Object doesn't support this property or method");
    public static VBRuntimeErrorException AutomationError => new(440, "Automation error");
    public static VBRuntimeErrorException RemoteProcessConnectionLost => new(442, "Connection to type library or object library for remote process has been lost. Press OK for dialog to remove reference.");
    public static VBRuntimeErrorException AutomationObjectHasNoDefaultValue => new(443, "Automation object does not have a default value");
    public static VBRuntimeErrorException UnsupportedObjectAction => new(445, "Object doesn't support this action");
    public static VBRuntimeErrorException UnsupportedObjectNamedArguments => new(446, "Object doesn't support named arguments");
    public static VBRuntimeErrorException UnsupportedObjectLocaleSetting => new(447, "Object doesn't support current locale setting");
    public static VBRuntimeErrorException NamedArgumentNotFound => new(448, "Named argument not found");
    public static VBRuntimeErrorException ArgumentNotOptional => new(449, "Argument not optional");
    public static VBRuntimeErrorException WrongNumberOfArgumentsOrInvalidPropertyAssignment => new(450, "Wrong number of arguments or invalid property assignment");
    public static VBRuntimeErrorException PropertyLetNotDefinedPropertyGetNotAnObject => new(451, "Property let procedure not defined and property get procedure did not return an object");
    public static VBRuntimeErrorException InvalidOrdinal => new(452, "Invalid ordinal");
    public static VBRuntimeErrorException DllFunctionNotFound => new(453, "Specified DLL function not found");
    public static VBRuntimeErrorException CodeResourceNotFound => new(454, "Code resource not found");
    public static VBRuntimeErrorException CodeResourceLockError => new(455, "Code resource lock error");
    public static VBRuntimeErrorException KeyAlreadyExists => new(457, "This key is already associated with an element of this collection");
    public static VBRuntimeErrorException UnsupportedAutomationType => new(458, "Variable uses an Automation type not supported in Visual Basic");
    public static VBRuntimeErrorException UnsupportedSetOfEvents => new(459, "Object or class does not support the set of events.");
    public static VBRuntimeErrorException InvalidClipboardFormat => new(460, "Invalid clipboard format");
    public static VBRuntimeErrorException MethodOrDataMemberNotFound => new(461, "Method or data member not found");
    public static VBRuntimeErrorException RemoteMachineUnavailable => new(462, "The remote machine does not exist or is unavailable");
    public static VBRuntimeErrorException ClassNotRegistered => new(463, "Class not registered on local machine");
    public static VBRuntimeErrorException InvalidPicture => new(481, "Invalid picture");
    public static VBRuntimeErrorException PrinterError => new(482, "Printer error");
    public static VBRuntimeErrorException CantSaveFileToTemp => new(735, "Can't save file to TEMP");
    public static VBRuntimeErrorException SearchTextNotFound => new(744, "Search text not found");
    public static VBRuntimeErrorException ReplacementsTooLong => new(746, "Replacements too long");

    public static VBRuntimeErrorException ApplicationDefinedError(int number = 1004) => new(number, "Application-defined or object-defined error");

    public VBRuntimeErrorException(int vBErrorNumber, string message, string? verbose = null)
        : base($"Runtime error '{vBErrorNumber}': {message}")
    {
        VBErrorNumber = vBErrorNumber;
        Verbose = verbose;
    }

    public int VBErrorNumber { get; }
    public string? Verbose { get; }

    public (int, string) Deconstruct(out int vbErrorNumber, out string message) => 
        (vbErrorNumber = VBErrorNumber, message = Message);

    public (int, string, string?) Deconstruct(out int vbErrorNumber, out string message, out string? verbose) => 
        (vbErrorNumber = VBErrorNumber, message = Message, verbose = Verbose);
}
