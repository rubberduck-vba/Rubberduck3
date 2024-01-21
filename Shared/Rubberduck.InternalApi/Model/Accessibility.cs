namespace Rubberduck.InternalApi.Model;

//Needed for VB6 support, although 1 and 2 are applicable to VBA.  See 5.2.4.1.1 https://msdn.microsoft.com/en-us/library/ee177292.aspx
//and 5.2.4.1.2 https://msdn.microsoft.com/en-us/library/ee199159.aspx
public enum Instancing
{
    Private = 1,                    //Not exposed via COM.
    PublicNotCreatable = 2,         //TYPEFLAG_FCANCREATE not set.
    SingleUse = 3,                  //TYPEFLAGS.TYPEFLAG_FAPPOBJECT
    GlobalSingleUse = 4,
    MultiUse = 5,
    GlobalMultiUse = 6
}

public enum Accessibility
{
    Implicit,
    Private,
    Friend,
    Public,
    Global,
    Static
}

public static class AccessibilityExtensions
{
    /// <summary>
    /// Gets the string/token representation of an accessibility specifier.
    /// </summary>
    /// <remarks>Implicit accessibility being unspecified, yields an empty string.</remarks>
    public static string TokenString(this Accessibility access)
    {
        return access == Accessibility.Implicit ? string.Empty : access.ToString();
    }
}
