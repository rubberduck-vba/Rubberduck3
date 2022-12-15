using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Linq;
using static Rubberduck.Parsing.Grammar.VBAParser;

namespace Rubberduck.Parsing.Model.Symbols
{
    public sealed class SubroutineDeclaration : ModuleBodyElementDeclaration
    {
        public SubroutineDeclaration(
            QualifiedMemberName name,
            Declaration parent,
            Declaration parentScope,
            string asTypeName,
            Accessibility accessibility,
            DocumentOffset offset,
            bool isUserDefined,
            IEnumerable<IParseTreeAnnotation> annotations,
            Attributes attributes)
            : base(
                  name,
                  parent,
                  parentScope,
                  asTypeName,
                  null,
                  string.Empty,
                  accessibility,
                  DeclarationType.Procedure,
                  offset,
                  false,
                  isUserDefined,
                  annotations,
                  attributes)
        { }

        public SubroutineDeclaration(ComMember member, Declaration parent, QualifiedModuleName module, Attributes attributes, bool eventHandler)
            : base(
                  module.QualifyMemberName(member.Name),
                  parent,
                  parent,
                  string.Empty,
                  null,
                  string.Empty,
                  Accessibility.Global,
                  eventHandler ? DeclarationType.Event : DeclarationType.Procedure,
                  DocumentOffset.Invalid,
                  false,
                  false,
                  null,
                  attributes)
        {
            AddParameters(member.Parameters.Select(decl => new ParameterDeclaration(decl, this, module)));
        }

        /// <inheritdoc/>
        protected override bool Implements(IInterfaceExposable member)
        {
            if (ReferenceEquals(member, this))
            {
                return false;
            }

            return DeclarationType == DeclarationType.Procedure
                   && member.DeclarationType == DeclarationType.Procedure
                   && IdentifierName.Equals(member.ImplementingIdentifierName)
                   && member.IsInterfaceMember
                   && ((ClassModuleDeclaration)member.ParentDeclaration).Subtypes.Any(implementation => ReferenceEquals(implementation, ParentDeclaration));
        }
    }
}
