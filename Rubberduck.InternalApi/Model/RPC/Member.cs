namespace Rubberduck.InternalApi.Model.RPC
{
    public class Member
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public Declaration ImplementsDeclaration { get; set; }
        public Accessibility Accessibility { get; set; }
        public bool IsAutoAssigned { get; set; }
        public bool IsWithEvents { get; set; }
        public bool IsDimStmt { get; set; }
        public string ValueExpression { get; set; }
    }
}
