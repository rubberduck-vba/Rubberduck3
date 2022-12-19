namespace Rubberduck.InternalApi.Model.RPC
{
    public class Local
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public bool IsAutoAssigned { get; set; }
        public string ValueExpression { get; set; }
    }
}
