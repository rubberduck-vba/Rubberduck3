namespace Rubberduck.InternalApi.Model.RPC
{
    public class Parameter
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public int Position { get; set; }
        public bool IsParamArray { get; set; }
        public bool IsOptional { get; set; }
        public bool IsByRef { get; set; }
        public bool IsByVal { get; set; }
        public bool IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }
    }
}
