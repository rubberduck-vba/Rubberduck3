namespace Rubberduck.Parsing.Listeners;

public interface IBlockFoldingSettings
{
    bool FoldModuleHeader { get; set; }
    bool FoldModuleAttributes { get; set; }
    bool FoldModuleDeclarations { get; set; }
    bool FoldProcedures { get; set; }
    bool FoldPropertyGroups { get; set; }
    bool FoldBlockStatements { get; set; }
    bool FoldEnumDeclarations { get; set; }
    bool FoldTypeDeclarations { get; set; }
}
