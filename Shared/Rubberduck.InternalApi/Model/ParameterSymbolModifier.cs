namespace Rubberduck.InternalApi.Model
{
    public record class ParameterSymbolModifier
    {
        public ParameterSymbolModifier(ParameterModifier modifier)
        {
            Value = modifier;
        }

        public ParameterModifier Value { get; init; }

        public bool IsByVal => Value == ParameterModifier.ExplicitByVal
                            || Value == ParameterModifier.ImplicitByVal;

        public bool IsByRef => Value == ParameterModifier.ExplicitByRef
                            || Value == ParameterModifier.ImplicitByRef;

        public bool IsImplicit => Value == ParameterModifier.ImplicitByVal
                            || Value == ParameterModifier.ImplicitByRef;

        public bool IsExplicit => Value == ParameterModifier.ExplicitByVal
                            || Value == ParameterModifier.ExplicitByRef;
    }
}
