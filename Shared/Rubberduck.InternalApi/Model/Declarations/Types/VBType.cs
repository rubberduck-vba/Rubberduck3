using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBEmptyType : VBType<int?>
    {
        private VBEmptyType() : base(Tokens.Empty) { }
        public static VBEmptyType TypeInfo { get; } = new();

        public override int? DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbBooleanType, VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBBooleanType : VBType<bool>
    {
        private VBBooleanType() : base(Tokens.Boolean) { }
        public static VBBooleanType TypeInfo { get; } = new();

        public override bool DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbByteType, VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBByteType : VBType<byte>
    {
        private VBByteType() : base(Tokens.Byte) { }
        public static VBByteType TypeInfo { get; } = new();

        public override byte DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbIntegerType, VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBIntegerType : VBType<short>
    {
        private VBIntegerType() : base(Tokens.Integer) { }
        public static VBIntegerType TypeInfo { get; } = new();

        public override short DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbLongType, VbLongLongType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBLongType : VBType<int>
    {
        private VBLongType() : base(Tokens.Long) { }
        public static VBLongType TypeInfo { get; } = new();

        public override int DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbLongLongType, VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBLongLongType : VBType<long>
    {
        private VBLongLongType() : base(Tokens.LongLong) { }
        public static VBLongLongType TypeInfo { get; } = new();

        public override long DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbDecimalType, VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBCurrencyType : VBType<decimal>
    {
        private VBCurrencyType() : base(Tokens.Currency) { }
        public static VBCurrencyType TypeInfo { get; } = new();

        public override decimal DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbDecimalType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBDecimalType : VBType<decimal>
    {
        private VBDecimalType() : base(Tokens.Decimal) { }
        public static VBDecimalType TypeInfo { get; } = new();

        public override decimal DefaultValue { get; }

        public override bool IsDeclarable { get; } = false; // indeed, "As Decimal" is illegal, ..but CDec() returns a Decimal.

        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbCurrencyType, VbSingleType, VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBSingleType : VBType<float>
    {
        private VBSingleType() : base(Tokens.Single) { }
        public static VBSingleType TypeInfo { get; } = new();

        public override float DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbDoubleType, VbStringType, VbVariantType];
    }

    public record class VBDoubleType : VBType<double>
    {
        private VBDoubleType() : base(Tokens.Double) { }
        public static VBDoubleType TypeInfo { get; } = new();

        public override double DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; } 
            = [VbStringType, VbVariantType];
    }

    public record class VBDateType : VBType<DateTime>
    {
        private VBDateType() : base(Tokens.Date) { }
        public static VBDateType TypeInfo { get; } = new();

        public override DateTime DefaultValue { get; } = new DateTime(1899, 12, 30);
        public override VBType[] ConvertsSafelyToTypes { get; } 
            = [VbStringType, VbVariantType];
    }

    public record class VBStringType : VBType<string?>
    {
        public static string? VBNullString { get; } = null;

        private VBStringType() : base(Tokens.String) { }
        public static VBStringType TypeInfo { get; } = new();

        public override string? DefaultValue { get; } = VBNullString;
        public override VBType[] ConvertsSafelyToTypes { get; } 
            = [VbVariantType];
    }

    public record class VBObjectType : VBType<object?>
    {
        public static object? Nothing { get; } = null;

        private VBObjectType() : base(Tokens.Object) { }
        public static VBObjectType TypeInfo { get; } = new();

        public override object? DefaultValue { get; } = Nothing;
        public override VBType[] ConvertsSafelyToTypes { get; } 
            = [VbVariantType];
    }

    public record class VBVariantType : VBType<object?>
    {
        public static object? VBEmpty { get; } = null;

        private VBVariantType(VBType? subtype = null) : base(Tokens.Variant) 
        {
            Subtype = subtype ?? VbEmptyType;
        }

        public VBType Subtype { get; init; }
        public bool IsEmpty => Subtype == VbEmptyType;

        public static VBVariantType TypeInfo { get; } = new();

        public override object? DefaultValue { get; } = VBEmpty;
        public override VBType[] ConvertsSafelyToTypes { get; } = [];
    }

    public record class VBLongPtrType : VBType<int>
    {
        public static int Size { get; } = sizeof(int); // FIXME use bitness from client info

        public VBLongPtrType() : base(Tokens.LongPtr) { }
        public static VBLongPtrType TypeInfo { get; } = new();

        public override int DefaultValue { get; }
        public override VBType[] ConvertsSafelyToTypes { get; }
            = [VbLongType, VbLongLongType];
    }

    public abstract record class VBType<T> : VBType
    {
        protected VBType(string name, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false) 
            : base(typeof(T), name, declaration, definitions, isUserDefined)
        {
        }

        public abstract VBType[] ConvertsSafelyToTypes { get; }
        public abstract T? DefaultValue { get; }
    }

    public record class VBClassType : VBType<object?>
    {
        public VBClassType(string name, Uri uri, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false)
            : base(name, declaration, definitions, isUserDefined) 
        {
            Uri = uri;
        }

        public Uri Uri { get; }

        public override VBType[] ConvertsSafelyToTypes => Supertypes.Concat([VbVariantType]).ToArray();
        public override object? DefaultValue { get; } = VBObjectType.Nothing;
    }

    public record class VBUserDefinedType : VBType<object>
    {
        public VBUserDefinedType(string name, Uri uri, Symbol? declaration = null, Symbol[]? defininitions = null)
            : base(name, declaration, defininitions, isUserDefined: true)
        {
            Uri = uri;
            DefaultValue = new();
        }

        public Uri Uri { get; }

        public override VBType[] ConvertsSafelyToTypes { get; } = [VbVariantType];
        public override object DefaultValue { get; }

        public override bool CanPassByValue { get; } = false;
    }

    public record class VBEnumType : VBType<int>
    {
        public VBEnumType(string name, Uri uri, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false)
            : base(name, declaration, definitions, isUserDefined)
        {
            Uri = uri;
        }

        public Uri Uri { get; }

        public override VBType[] ConvertsSafelyToTypes { get; } = [VbIntegerType, VbLongType, VbLongLongType, VbVariantType];
        public override int DefaultValue { get; }
    }

    public abstract record class VBType
    {
        #region intrinsic types
        public static VBType VbBooleanType { get; } = VBBooleanType.TypeInfo;
        public static VBType VbByteType { get; } = VBByteType.TypeInfo;
        public static VBType VbIntegerType { get; } = VBIntegerType.TypeInfo;
        public static VBType VbLongType { get; } = VBLongType.TypeInfo;
        public static VBType VbLongPtrType { get; } = VBLongPtrType.TypeInfo;
        public static VBType VbLongLongType { get; } = VBLongLongType.TypeInfo;
        public static VBType VbCurrencyType { get; } = VBCurrencyType.TypeInfo;
        public static VBType VbDecimalType { get; } = VBDecimalType.TypeInfo;
        public static VBType VbSingleType { get; } = VBSingleType.TypeInfo;
        public static VBType VbDoubleType { get; } = VBDoubleType.TypeInfo;
        public static VBType VbDateType { get; } = VBDateType.TypeInfo;
        public static VBType VbStringType { get; } = VBStringType.TypeInfo;
        public static VBType VbObjectType { get; } = VBObjectType.TypeInfo;
        public static VBType VbVariantType { get; } = VBVariantType.TypeInfo;
        public static VBType VbEmptyType { get; } = VBEmptyType.TypeInfo;

        public static ImmutableArray<VBType> IntrinsicTypes { get; } =
            [
                VbBooleanType,
                VbByteType,
                VbIntegerType,
                VbLongType,
                VbLongLongType,
                VbCurrencyType,
                VbDecimalType,
                VbSingleType,
                VbDoubleType,
                VbDateType,
                VbStringType,
                VbObjectType,
                VbVariantType,
            ];

        public static bool TryResolveIntrinsic(string? typeName, out VBType? type)
        {
            type = IntrinsicTypes.SingleOrDefault(t => t.Name.Equals(typeName ?? Tokens.Variant, StringComparison.InvariantCultureIgnoreCase));
            return type != null;
        }
        #endregion

        public VBType(string name, Symbol declaration, VBType[] supertypes, VBType[] subtypes)
            : this(typeof(object), name, declaration, [declaration], isUserDefined: false)
        {
            Supertypes = supertypes;
            Subtypes = subtypes;
        }

        public VBType(string name, Symbol declaration, VBType[]? subtypes = default, bool isUserDefined = default)
            : this(null, name, declaration, [declaration], isUserDefined) 
        {
            Subtypes = subtypes ?? [];
        }

        public VBType(Type? managedType, string name, Symbol? declaration = default, Symbol[]? definitions = default, bool isUserDefined = false)
        {
            ManagedType = managedType;
            Name = name;
            Declaration = declaration;
            Definitions = definitions ?? [];
            IsUserDefined = isUserDefined;
        }

        public Type? ManagedType { get; init; }

        public string Name { get; init; }
        public Symbol? Declaration { get; init; }
        public Symbol[] Definitions { get; init; } = [];

        public bool IsIntrinsic => Definitions.Length == 0;
        public bool IsUserDefined { get; init; }
        public bool IsArray { get; init; }
        public virtual bool CanPassByValue => !IsArray;
        public virtual bool IsDeclarable => true;

        public bool IsInterface => Subtypes.Length != 0;

        public VBType[] Supertypes { get; init; } = [];
        public VBType[] Subtypes { get; init; } = [];
    }
}
