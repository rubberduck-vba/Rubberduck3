using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Model
{
    public class ParameterInfo : TypedNamedOffsetInfo
    {
        public ParameterInfo(int ordinal, VBAParser.ArgContext context)
            : base(name: context.unrestrictedIdentifier()?.identifier()?.GetText() ?? string.Empty, 
                   offset: context.Offset, 
                   typeName: context.asTypeClause()?.type()?.GetText() ?? Tokens.Variant, 
                   hasExplicitType: context.asTypeClause() != null)
        {
            Ordinal = ordinal;
            
            IsArray = context.LPAREN() != null && context.RPAREN() != null;
            IsOptional = context.OPTIONAL() != null;
            IsParamArray = context.PARAMARRAY() != null;
            IsByRefExplicit = context.BYREF() != null;
            IsByValExplicit = context.BYVAL() != null;

            var isPropertyValueArg = false;
            switch (context.Parent.Parent)
            {
                case VBAParser.PropertyLetStmtContext _:
                case VBAParser.PropertySetStmtContext _:
                    isPropertyValueArg = ordinal == 1;
                    break;
            }

            IsByRef = !isPropertyValueArg && !IsByValExplicit;

            DefaultValueExpression = context.argDefaultValue()?.expression()?.GetText();
        }

        public int Ordinal { get; }
        public bool IsByRef { get; }
        public bool IsByRefExplicit { get; }
        public bool IsByVal => !IsByRef || IsByValExplicit;
        public bool IsByValExplicit { get; }
        public bool IsArray { get; }
        public bool IsParamArray { get; }


        public bool IsOptional { get; }
        public string DefaultValueExpression { get; }
    }
}
