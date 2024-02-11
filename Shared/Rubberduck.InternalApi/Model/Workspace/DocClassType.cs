namespace Rubberduck.InternalApi.Model.Workspace;

/// <summary>
/// An enumeration used for identifying the type of a VBA document class
/// </summary>
/// <remarks>
/// Mirrors the <c>Rubberduck.Unmanaged.TypeLibs.Utility.DocClassType</c> enum type.
/// </remarks>
public enum DocClassType
{
    Unrecognized = 0,
    ExcelWorkbook = 1,
    ExcelWorksheet = 2,
    AccessForm = 3,
    AccessReport = 4,
}
