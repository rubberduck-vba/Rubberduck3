using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    public abstract class PropertyDeclaration : ModuleBodyElementDeclaration
    {
        protected PropertyDeclaration(
            QualifiedMemberName name,
            Declaration parent,
            Declaration parentScope,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            Accessibility accessibility,
            DeclarationType type,
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
                type,
                offset,
                isArray,
                isUserDefined,
                annotations,
                attributes)
        { }

        public override bool IsObject
        {
            get
            {
                if (base.IsObject)
                {
                    return true;
                }

                return (DeclarationType == DeclarationType.PropertyLet ||
                       DeclarationType == DeclarationType.PropertySet) &&
                       (Parameters.OrderBy(p => p.Offset.Start).LastOrDefault()?.IsObject ?? false);
            }
        }

        /// <inheritdoc/>
        protected abstract override bool Implements(IInterfaceExposable member);
    }
}
