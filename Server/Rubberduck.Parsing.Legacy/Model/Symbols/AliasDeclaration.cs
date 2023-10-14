﻿using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class AliasDeclaration : Declaration
    {
        public AliasDeclaration(ComAlias alias, Declaration parent, QualifiedModuleName module)
            : base(
                module.QualifyMemberName(alias.Name),
                parent,
                parent,
                alias.TypeName,
                null,
                false,
                false,
                Accessibility.Public,
                DeclarationType.ComAlias,
                DocumentOffset.Invalid,
                false,
                true,
                false)
        { }
    }
}
