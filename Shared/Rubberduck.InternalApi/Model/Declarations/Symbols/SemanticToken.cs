using Rubberduck.InternalApi.ServerPlatform.LanguageServer;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class SemanticToken
{
    public int Id { get; }
    public RubberduckSemanticTokenType Type { get; }
    public RubberduckSemanticTokenModifier Modifier { get; }
}
