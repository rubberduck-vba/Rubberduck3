using System;

namespace Rubberduck.InternalApi.Common;

public readonly struct WindowsVersion : IComparable<WindowsVersion>, IEquatable<WindowsVersion>
{
    public static readonly WindowsVersion Windows11_22H2 = new(10, 0, 22621); // 22H2 2022-09-20 to 2025-10-14
    public static readonly WindowsVersion Windows11_21H2 = new(10, 0, 22000); // 21H2 2021-10-04 to 2024-10-08
    // out of support:
    public static readonly WindowsVersion Windows10 = new(10, 0, 10240);
    public static readonly WindowsVersion Windows81 = new(6, 3, 9200);
    public static readonly WindowsVersion Windows8 = new(6, 2, 9200);
    public static readonly WindowsVersion Windows7_SP1 = new(6, 1, 7601);
    public static readonly WindowsVersion WindowsVista_SP2 = new(6, 0, 6002);

    public WindowsVersion(int major, int minor, int build)
    {
        Major = major;
        Minor = minor;
        Build = build;
    }

    public int Major { get; init; }
    public int Minor { get; init; }
    public int Build { get; init; }


    public readonly int CompareTo(WindowsVersion other)
    {
        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0)
        {
            return majorComparison;
        }

        var minorComparison = Minor.CompareTo(other.Minor);

        return minorComparison != 0
            ? minorComparison
            : Build.CompareTo(other.Build);
    }

    public readonly bool Equals(WindowsVersion other) => Major == other.Major && Minor == other.Minor && Build == other.Build;

    public override readonly bool Equals(object? other) => other is WindowsVersion otherVersion && Equals(otherVersion);

    public override readonly int GetHashCode() => HashCode.Combine(Major, Minor, Build);

    public static bool operator ==(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) == 0;

    public static bool operator !=(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) != 0;

    public static bool operator <(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) < 0;

    public static bool operator >(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) > 0;

    public static bool operator <=(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) <= 0;

    public static bool operator >=(WindowsVersion os1, WindowsVersion os2) => os1.CompareTo(os2) >= 0;
}
