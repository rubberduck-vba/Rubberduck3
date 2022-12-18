using Antlr4.Runtime;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.Parsing.Grammar;
using static Rubberduck.Parsing.Grammar.VBAParser;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model.Symbols
{
    public sealed class PropertySetDeclaration : PropertyDeclaration
    {
        public PropertySetDeclaration(
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
                null,
                accessibility,
                DeclarationType.PropertySet,
                offset,
                false,
                isUserDefined,
                annotations,
                attributes)
        { }

        public PropertySetDeclaration(ComMember member, Declaration parent, QualifiedModuleName module, Attributes attributes) 
            : this(
                module.QualifyMemberName(member.Name),
                parent,
                parent,
                member.AsTypeName.TypeName,
                Accessibility.Global,
                DocumentOffset.Invalid,
                false,
                null,
                attributes)
        {
            AddParameters(member.Parameters.Select(decl => new ParameterDeclaration(decl, this, module)));
        }

        public PropertySetDeclaration(ComField field, Declaration parent, QualifiedModuleName module, Attributes attributes) 
            : this(
                module.QualifyMemberName(field.Name),
                parent,
                parent,
                field.ValueType,
                Accessibility.Global,
                DocumentOffset.Invalid,
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

            return member.IsInterfaceMember
                   && IdentifierName.Equals(member.ImplementingIdentifierName)
                   && ((ClassModuleDeclaration)member.ParentDeclaration).Subtypes.Any(implementation => ReferenceEquals(implementation, ParentDeclaration))
                   && (member.DeclarationType == DeclarationType.PropertySet
                       || member.DeclarationType == DeclarationType.Variable 
                       && (member.IsObject || member.AsTypeName.Equals(Tokens.Variant)));
        }
    }
}
