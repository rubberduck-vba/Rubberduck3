namespace Rubberduck.RPC.Platform.Model.LocalDb
{
    public class ProjectSetting
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int DataType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
