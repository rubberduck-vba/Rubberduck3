namespace Rubberduck.InternalApi.RPC.DataServer
{

    public class DeclarationAttribute
    {
        public int Id { get; set; }
        public int DeclarationId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValues { get; set; }
        
        public LocationInfo LocationInfo { get; set; }
    }
}
