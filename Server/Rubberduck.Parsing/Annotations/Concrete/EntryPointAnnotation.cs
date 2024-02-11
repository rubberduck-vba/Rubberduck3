using Rubberduck.InternalApi.Model.Declarations;

namespace Rubberduck.Parsing.Annotations.Concrete;

/// <summary>
/// @EntryPoint annotation, marks a standard or document module member as an entry point procedure that is not intended to be referenced directly from the code.
/// </summary>
/// <parameter name="CallerName" type="Text" required="False">
/// If provided, the first argument is interpreted as referring to an external caller, for example the name of a Shape in the host document.
/// </parameter>
/// <remarks>
/// Members with this annotation are ignored by the ProcedureNotUsed inspection. The CallerName argument is currently not being validated, but may be in the future. 
/// When hosted in Microsoft Excel, the @ExcelHotkey annotation can be used in standard modules instead of @EntryPoint to associate a hotkey shortcut.
/// </remarks>
/// <example>
/// <module name="Module1" type="Standard Module">
/// <![CDATA[
/// Option Explicit
/// Option Private Module
///
/// '@EntryPoint
/// Public Sub DoSomething()
///     '...
/// End Sub
/// 
/// '@EntryPoint "Rounded Rectangle 1"
/// Public Sub DoSomethingElse()
///     '...
/// End Sub
/// ]]>
/// </module>
/// </example>
public sealed class EntryPointAnnotation : AnnotationBase
{
    public string CallerName { get; private set; } = string.Empty;

    public EntryPointAnnotation()
        : base("EntryPoint", AnnotationTarget.Member, allowedArguments: 1, allowedArgumentTypes: new[] { AnnotationArgumentType.Text })
    { }

    // annotation is legal in ComponentType.StandardModule and ComponentType.Document modules.

    public override IReadOnlyList<ComponentKind> IncompatibleComponentKinds => [
        ComponentKind.ActiveXDesigner,
        ComponentKind.ClassModule,
        ComponentKind.ComComponent,
        ComponentKind.DocObject,
        ComponentKind.MDIForm,
        ComponentKind.PropertyPage,
        ComponentKind.RelatedDocument,
        ComponentKind.ResourceFile,
        ComponentKind.Undefined,
        ComponentKind.UserControl,
        ComponentKind.UserFormModule,
        ComponentKind.VBForm,
    ];

    public override IReadOnlyList<string> ProcessAnnotationArguments(IEnumerable<string> arguments)
    {
        var args = arguments.ToList();

        CallerName = args.Any()
            ? args[0]
            : string.Empty;

        return base.ProcessAnnotationArguments(args);
    }
}
