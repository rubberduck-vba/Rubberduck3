namespace Rubberduck.Settings
{
    public class BlockFoldingSettings //: IBlockFoldingSettings
    {
        public bool FoldModuleHeader { get; set; } = true;
        public bool FoldModuleAttributes { get; set; } = true;
        public bool FoldModuleDeclarations { get; set; } = true;
        public bool FoldProcedures { get; set; } = true;
        public bool FoldPropertyGroups { get; set; }
        public bool FoldBlockStatements { get; set; } = true;
        public bool FoldEnumDeclarations { get; set; } = true; 
        public bool FoldTypeDeclarations { get; set; } = true;
    }
}