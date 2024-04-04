namespace Rubberduck.Unmanaged
{
    public readonly struct CommandBarLocation
    {
        public CommandBarLocation(string parentName, int beforeControlId)
        {
            ParentName = parentName;
            ParentId = default;
            BeforeControlId = beforeControlId;
        }

        public CommandBarLocation(int parentId, int beforeControlId)
        {
            ParentName = default;
            ParentId = parentId;
            BeforeControlId = beforeControlId;
        }

        public string ParentName { get; }
        public int ParentId { get; }
        public int BeforeControlId { get; }
    }
}
