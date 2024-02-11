using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Execution.Values;

public abstract record class VBArrayValue : VBTypedValue
{
    public VBArrayValue((int lBound, int uBound)[] dimensions, VBType? itemType = null, TypedSymbol? symbol = null)
        : base(VBArrayType.TypeInfo, symbol)
    {
        ItemType = itemType ?? VBVariantType.TypeInfo;
        Dimensions = dimensions.Select(e => new VBArrayDimension(symbol, ItemType, e.uBound, e.lBound)).ToImmutableArray();
    }

    public bool IsParamArray { get; init; }

    public override int Size => ItemType.DefaultValue.Size * Dimensions.Length;
    public VBType ItemType { get; init; }

    public ImmutableArray<VBArrayDimension> Dimensions { get; init; }
    public bool IsInitialized => Dimensions.Length > 0;

    public abstract bool CanAssignToArray(VBArrayValue value);
    public VBArrayValue Erase() => this with { Dimensions = [] };

    public record class VBArrayDimension
    {
        private readonly Symbol? _symbol;

        public VBArrayDimension(Symbol? symbol, VBType itemType, int uBound, int lBound)
        {
            UpperBound = uBound;
            LowerBound = lBound;

            _symbol = symbol;
            State = new VBTypedValue[Length].Select(e => itemType.DefaultValue).ToArray();
        }

        public VBTypedValue[] State { get; init; }

        public VBTypedValue this[int index]
        {
            get
            {
                if (index < LowerBound || index > UpperBound)
                {
                    if (_symbol != null)
                    {
                        throw VBRuntimeErrorException.SubscriptOutOfRange(_symbol);
                    }
                }
                else
                {
                    return State[index];
                }

                throw new IndexOutOfRangeException(); // that would be a bug
            }
        }

        public int UpperBound { get; init; }
        public int LowerBound { get; init; }

        public int Length => UpperBound - LowerBound + 1;
        public bool IsInitialized => LowerBound < UpperBound;

        public void Deconstruct(out int lBound, out int uBound)
        {
            lBound = LowerBound;
            uBound = UpperBound;
        }
    }
}

public record class VBFixedSizeArrayValue : VBArrayValue
{
    public VBFixedSizeArrayValue(int uBound, int lBound, VBType? itemType = null, TypedSymbol? symbol = null)
        : this([(uBound, lBound)], itemType, symbol)
    {
    }
    public VBFixedSizeArrayValue((int uBound, int lBound)[] dimensions, VBType? itemType = null, TypedSymbol? symbol = null)
        : base(dimensions, itemType, symbol)
    {
    }

    /// <remarks>
    /// This could be an expensive check to make, calling it should issue a perf diagnostic.
    /// </remarks>
    public override bool CanAssignToArray(VBArrayValue value) =>
        value.Dimensions.SequenceEqual(Dimensions)
        && value.Dimensions.All(d => d.State.All(state => state == state.TypeInfo.DefaultValue));
}

public record class VBFixedSizeArrayValue<VBT> : VBFixedSizeArrayValue where VBT : VBType
{
    public VBFixedSizeArrayValue(int uBound, int lBound, VBType? itemType = null, TypedSymbol? symbol = null)
        : base([(uBound, lBound)], itemType, symbol)
    {
    }
    public VBFixedSizeArrayValue((int uBound, int lBound)[] dimensions, VBType? itemType = null, TypedSymbol? symbol = null)
        : base(dimensions, itemType, symbol)
    {
    }
}

public record class VBResizableArrayValue : VBArrayValue
{
    public VBResizableArrayValue(int uBound, int lBound, VBType? itemType = null, TypedSymbol? symbol = null)
        : this([(uBound, lBound)], itemType, symbol)
    {
    }

    public VBResizableArrayValue((int uBound, int lBound)[] dimensions, VBType? itemType = null, TypedSymbol? symbol = null)
        : base(dimensions, itemType, symbol)
    {
    }

    public override bool CanAssignToArray(VBArrayValue value) => !value.Dimensions.Any();

    public VBArrayValue ReDim((int lBound, int uBound)[] dimensions, bool preserve = false)
    {
        if (IsWithBlockVariable)
        {
            throw VBCompileErrorException.InvalidReDim(Symbol!, "The target of a `ReDim` statement cannot be a `With` block variable.");
        }

        if (IsParamArray)
        {
            throw VBCompileErrorException.InvalidParamArrayUse(Symbol!, "Parameter array value cannot be resized.");
        }

        if (preserve)
        {
            if (dimensions.Length != Dimensions.Length)
            {
                throw VBRuntimeErrorException.SubscriptOutOfRange(Symbol!, "`ReDim Preserve` cannot change the number of dimensions of a resizable array.");
            }

            for (var i = 0; i < dimensions.Length; i++)
            {
                var (oldLower, oldUpper) = Dimensions[i];
                var (newLower, newUpper) = dimensions[i];

                if (i != dimensions.Length - 1)
                {
                    if (oldLower != newLower)
                    {
                        throw VBRuntimeErrorException.SubscriptOutOfRange(Symbol!, "`ReDim Preserve` cannot change the lower boundary of a resizable array.");
                    }

                    if (oldUpper != newUpper)
                    {
                        throw VBRuntimeErrorException.SubscriptOutOfRange(Symbol!, "`ReDim Preserve` cannot change the upper boundary of a dimension that isn't the last dimension of a resizable array.");
                    }

                    if (newUpper < oldUpper)
                    {
                        // issue diagnostic for data truncation?
                    }
                }
            }
        }

        if (preserve)
        {
            var oldDim = Dimensions[^0];

            var newUpperBound = dimensions[^0].uBound;
            var newDim = new VBArrayDimension(Symbol, ItemType, newUpperBound, oldDim.LowerBound);

            var newState = oldDim.State.Take(newDim.Length).ToArray();
            if (newState.Length < newDim.Length)
            {
                newState = newState.Concat(Enumerable.Repeat(ItemType.DefaultValue, newDim.Length - newState.Length)).ToArray();
            }

            newDim = newDim with { State = newState.ToArray() };
            var newDimensions = Dimensions[..^1].Append(newDim);
            return this with { Dimensions = newDimensions.ToImmutableArray() };
        }

        return this with { Dimensions = dimensions.Select(e => new VBArrayDimension(Symbol, ItemType, e.uBound, e.lBound)).ToImmutableArray() };
    }
}

public record class VBResizableArrayValue<VBT> : VBResizableArrayValue where VBT : VBType
{
    public VBResizableArrayValue(int uBound, int lBound, VBType? itemType = null, TypedSymbol? symbol = null)
        : this([(uBound, lBound)], itemType, symbol)
    {
    }

    public VBResizableArrayValue((int uBound, int lBound)[] dimensions, VBType? itemType = null, TypedSymbol? symbol = null)
        : base(dimensions, itemType, symbol)
    {
    }
}
