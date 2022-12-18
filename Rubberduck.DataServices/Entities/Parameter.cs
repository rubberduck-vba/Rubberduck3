namespace Rubberduck.DataServices.Entities
{
    internal class Parameter : DbEntity
    {
        public int DeclarationId { get; set; }
        public int Position { get; set; }
        public int IsParamArray { get; set; }
        public int IsOptional { get; set; }
        public int IsByRef { get; set; }
        public int IsByVal { get; set; }
        public int IsModifierImplicit { get; set; }
        public string DefaultValue { get; set; }
    }
}
