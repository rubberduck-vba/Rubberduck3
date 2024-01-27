namespace Rubberduck.InternalApi.Model;

/// <summary>
/// Identifies a module type that can be represented with an icon.
/// </summary>
public enum ModuleType
{
    None,
    StandardModule,
    ClassModule,
    ClassModuleInterface,
    ClassModulePrivate,
    ClassModulePredeclared,
    DocumentModule,
    UserFormModule,
}
