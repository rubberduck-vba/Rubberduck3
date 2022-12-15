using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class ArgumentReference : IdentifierReference
    {
        internal ArgumentReference(
            QualifiedModuleName qualifiedName,
            Declaration parentScopingDeclaration,
            Declaration parentNonScopingDeclaration,
            string identifierName,
            DocumentOffset offset,
            VBABaseParserRuleContext context,
            VBAParser.ArgumentListContext argumentListContext,
            ArgumentListArgumentType argumentType,
            int argumentPosition,
            ParameterDeclaration referencedParameter,
            IEnumerable<IParseTreeAnnotation> annotations = null)
            : base(
                qualifiedName,
                parentScopingDeclaration,
                parentNonScopingDeclaration,
                identifierName,
                offset,
                context,
                referencedParameter,
                false,
                false,
                annotations)
        {
            ArgumentType = argumentType;
            ArgumentPosition = argumentPosition;
            ArgumentListContext = argumentListContext;
            NumberOfArguments = ArgumentListContext?.argument()?.Length ?? 0;
            ArgumentListSelection = argumentListContext?.Selection ?? Selection.Empty;
        }

        public ArgumentListArgumentType ArgumentType { get; }
        public int ArgumentPosition { get; }
        public int NumberOfArguments { get; }
        public VBAParser.ArgumentListContext ArgumentListContext { get; }
        public Selection ArgumentListSelection { get; }
    }
}