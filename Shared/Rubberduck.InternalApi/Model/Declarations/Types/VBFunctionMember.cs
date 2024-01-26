using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;

namespace Rubberduck.InternalApi.Model.Declarations.Types
{
    public record class VBFunctionMember : VBReturningMember
    {
        public VBFunctionMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol? declaration = null, Symbol[]? definitions = null, bool isUserDefined = false, VBType? type = null)
            : base(uri, name, kind, accessibility, declaration, definitions, isUserDefined, type)
        {
        }

        public VBFunctionMember(Uri uri, string name, RubberduckSymbolKind kind, Accessibility accessibility, Symbol declaration, Symbol[]? definitions = null, string? asTypeExpression = null, VBType? type = null)
            : base(uri, name, kind, accessibility, declaration, definitions, asTypeExpression, type)
        {
        }
    }
}
