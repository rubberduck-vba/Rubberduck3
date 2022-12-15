using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Linq;
using static Rubberduck.Parsing.Grammar.VBAParser;

namespace Rubberduck.Parsing.Model.Symbols
{
    public sealed class PropertyGetDeclaration : PropertyDeclaration
    {
        public PropertyGetDeclaration(
            QualifiedMemberName name,
            Declaration parent,
            Declaration parentScope,
            string asTypeName,
            AsTypeClauseContext asTypeContext,
            string typeHint,
            Accessibility accessibility,
            DocumentOffset offset,
            bool isArray,
            bool isUserDefined,
            IEnumerable<IParseTreeAnnotation> annotations,
            Attributes attributes)
            : base(
                  name,
                  parent,
                  parentScope,
                  asTypeName,
                  asTypeContext,
                  typeHint,
                  accessibility,
                  DeclarationType.PropertyGet,
                  offset,
                  isArray,
                  isUserDefined,
                  annotations,
                  attributes)
        { }

        public PropertyGetDeclaration(ComMember member, Declaration parent, QualifiedModuleName module, Attributes attributes)
            : this(
                module.QualifyMemberName(member.Name),
                parent,
                parent,
                member.AsTypeName.TypeName,
                null,
                null,
                Accessibility.Global,
                DocumentOffset.Invalid,
                member.AsTypeName.IsArray,
                false,
                null,
                attributes)
        {
            AddParameters(member.Parameters.Select(decl => new ParameterDeclaration(decl, this, module)));
        }

        public PropertyGetDeclaration(ComField field, Declaration parent, QualifiedModuleName module, Attributes attributes)
            : this(
                module.QualifyMemberName(field.Name),
                parent,
                parent,
                field.ValueType,
                null,
                null,
                Accessibility.Global,
                DocumentOffset.Invalid,
                false,  //TODO - check this assumption.
                false,
                null,
                attributes)
        { }

        /// <inheritdoc/>
        protected override bool Implements(IInterfaceExposable member)
        {
            if (ReferenceEquals(member, this))
            {
                return false;
            }

            return (member.DeclarationType == DeclarationType.PropertyGet || member.DeclarationType == DeclarationType.Variable)
                   && member.IsInterfaceMember
                   && ((ClassModuleDeclaration)member.ParentDeclaration).Subtypes.Any(implementation => ReferenceEquals(implementation, ParentDeclaration))
                   && IdentifierName.Equals(member.ImplementingIdentifierName);
        }
    }
}
