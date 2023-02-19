using System.Collections.Generic;
using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    /// <summary>
    /// These declarations are created from unresolved member accesses in the DeclarationFinder and are collected for use by inspections.  They
    /// should NOT be added to the Declaration collections in the parser state.
    /// </summary>
    public class UnboundMemberDeclaration : Declaration
    {
        /// <summary>
        /// Context on the LHS of the member access.
        /// </summary>
        /// <remarks>
        /// RD3.NOTE: Used in MemberNotOnInterfaceInspection to pass a Selection to the finder for identifier references.
        /// </remarks>
        //public ParserRuleContext CallingContext { get; private set; }

        public UnboundMemberDeclaration(Declaration parentDeclaration, VBABaseParserRuleContext unboundIdentifier, VBABaseParserRuleContext callingContext, IEnumerable<IParseTreeAnnotation> annotations) :
            base(new QualifiedMemberName(parentDeclaration.QualifiedMemberName.QualifiedModuleName, unboundIdentifier.GetText()),
                parentDeclaration,
                parentDeclaration,
                Tokens.Variant,
                string.Empty,
                false,
                false,
                Accessibility.Implicit,
                DeclarationType.UnresolvedMember,
                unboundIdentifier.Offset,
                false,
                false,
                true,
                annotations)
        {
            //CallingContext = callingContext;
        }
    }
}
