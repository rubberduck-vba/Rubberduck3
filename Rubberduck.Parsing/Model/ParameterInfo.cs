namespace Rubberduck.Parsing.Model
{
    public class ParameterInfo : TypedNamedOffsetInfo
    {
        public ParameterInfo(string name, DocumentOffset offset, string typeName, int ordinal, bool hasExplicitType, bool isByVal, bool isByRefExplicit, bool isOptional, bool isParamArray)
            : base(name, offset, typeName, hasExplicitType)
        {
            Ordinal = ordinal;
            IsByVal = isByVal;
            IsByRefExplicit = isByRefExplicit;
            IsOptional = isOptional;
            IsParamArray = isParamArray;
        }

        public int Ordinal { get; }
        public bool IsByVal { get; }
        public bool IsByRefExplicit { get; }
        public bool IsOptional { get; }
        public bool IsParamArray { get; }
    }
}
