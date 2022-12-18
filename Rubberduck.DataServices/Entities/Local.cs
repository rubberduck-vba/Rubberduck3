namespace Rubberduck.DataServices.Entities
{
    internal class Local : DbEntity
    {
        public int DeclarationId { get; set; }
        public int IsAutoAssigned { get; set; }
        public string ValueExpression { get; set; }
    }
}
