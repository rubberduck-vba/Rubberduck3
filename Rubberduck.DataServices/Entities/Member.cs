namespace Rubberduck.DataServices.Entities
{
    internal class Member : DbEntity
    {
        public int DeclarationId { get; set; }
        public int? ImplementsDeclarationId { get; set; }
        public int Accessibility { get; set; }
        public int IsAutoAssigned { get; set; }
        public int IsWithEvents { get; set; }
        public int IsDimStmt { get; set; }
        public string ValueExpression { get; set; }
    }
}
