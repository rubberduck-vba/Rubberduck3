using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types.Abstract;

public abstract record class VBIntrinsicType<T> : VBType
{
    protected VBIntrinsicType(string name) 
        : base(typeof(T), name, isUserDefined: false)
    {
    }

    /// <summary>
    /// Attemps to resolve the specified <c>typeName</c> expression to an intrinsic type.
    /// </summary>
    /// <returns>
    /// <c>true</c> if resolution was successful and <c>type</c> is not <c>null</c>.
    /// </returns>
    public static bool TryResolve(string? typeName, out VBType? type)
    {
        type = IntrinsicTypes.SingleOrDefault(t => t.Name.Equals(typeName ?? Tokens.Variant, StringComparison.InvariantCultureIgnoreCase));
        return type != null;
    }

    /// <summary>
    /// <c>true</c> for all intrinsic types that are valid in an <c>AsTypeClause</c> declaration.
    /// </summary>
    public virtual bool IsDeclarable { get; } = true;

    /// <summary>
    /// Specifies a <em>type hint</em> character that can help typing literal values.
    /// </summary>
    public virtual char? TypeHintCharacter { get; }

    /// <summary>
    /// Specifies a <c>DefType</c> token that could really put this type system to the test.
    /// </summary>
    /// <remarks>
    /// Probably best to not implement this?
    /// </remarks>
    public virtual string? DefToken { get; }
}
