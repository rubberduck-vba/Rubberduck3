namespace Rubberduck.InternalApi.RPC.DataServer
{
    public enum ProjectSettingDataType
    {
        Bool = 0,
        String = 1,
        Integer = 2,
        Float = 3,
    }

    public class ProjectSetting
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public Project Project { get; set; }
        public ProjectSettingDataType DataType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
