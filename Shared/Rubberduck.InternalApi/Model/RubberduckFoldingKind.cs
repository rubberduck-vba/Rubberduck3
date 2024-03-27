using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Rubberduck.InternalApi.Model;

public static class RubberduckFoldingKind
{
    public static readonly string HeaderFoldingKindName = "header";
    public static readonly string AttributeFoldingKindName = "attr";
    public static readonly string ScopeFoldingKindName = "scope";
    public static readonly string BlockFoldingKindName = "block";

    public static FoldingRangeKind ModuleHeader { get; } = new FoldingRangeKind(HeaderFoldingKindName);
    public static FoldingRangeKind Attributes { get; } = new FoldingRangeKind(AttributeFoldingKindName);
    public static FoldingRangeKind Scope { get; } = new FoldingRangeKind(ScopeFoldingKindName);
    public static FoldingRangeKind Block { get; } = new FoldingRangeKind(BlockFoldingKindName);
}
