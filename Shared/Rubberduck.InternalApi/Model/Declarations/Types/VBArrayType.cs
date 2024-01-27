using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

namespace Rubberduck.InternalApi.Model.Declarations.Types;

public record class VBArrayType : VBIntrinsicType<object[]>, IEnumerableType
{
    public const int ImplicitBoundary = -1;
    public static readonly VBArrayType Empty = new();

    public VBArrayType(VBType? subtype = null, int? lowerBound = null, int? upperBound = null) : base("Array")
    {
        Subtype = subtype ?? VbVariantType;
        DeclaredLowerBound = lowerBound ?? ImplicitBoundary;
        DeclaredUpperBound = upperBound ?? ImplicitBoundary;
    }

    public VBType Subtype { get; init; }

    public bool IsArray { get; } = true;

    public bool IsDynamic => DeclaredLowerBound is null && DeclaredUpperBound is null;
    public int? DeclaredLowerBound { get; init; }
    public int? DeclaredUpperBound { get; init; }

    public override object[]? DefaultValue => [];
    public override bool CanPassByValue { get; } = false;

    /// <summary>
    /// Creates and returns a copy of this <c>VBArray</c> type, with the specified bounds.
    /// </summary>
    /// <returns>
    /// <c>null</c> if <c>IsDynamic</c> is <c>false</c>, or if the <c>subtype</c> is specified but does not match the current declared subtype.
    /// </returns>
    /// <remarks>
    /// Compile error "Can't change the data types of array elements" (subtype mismatch)
    /// or "Array already dimensioned" (array is not dynamic) should be raised if <c>null</c> is returned.
    /// </remarks>
    public VBArrayType? ReDim(VBType? subtype = null, int ? lowerBound = null, int? upperBound = null)
        => IsDynamic && (subtype is null || subtype.Equals(Subtype))
            ? this with
                {
                    DeclaredLowerBound = lowerBound ?? DeclaredLowerBound,
                    DeclaredUpperBound = upperBound ?? DeclaredUpperBound,
                }
            : null;
}

