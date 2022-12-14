using System.Collections.Generic;
using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class ValuedDeclaration : Declaration
    {
        public ValuedDeclaration(
            QualifiedMemberName qualifiedName, 
            Declaration parentDeclaration,
            string parentScope,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            IEnumerable<IParseTreeAnnotation> annotations,
            Accessibility accessibility, 
            DeclarationType declarationType,
            string value,
            ParserRuleContext context, 
            Selection selection, 
            bool isUserDefined = true,
            ParserRuleContext attributesPassContext = null,
            Attributes attributes = null
            )
            :base(
                 qualifiedName, 
                 parentDeclaration, 
                 parentScope, 
                 asTypeName, 
                 typeHint,
                 true, 
                 false, 
                 accessibility,
                 declarationType, 
                 context, 
                 attributesPassContext,
                 selection,
                 false,
                 asTypeContext,
                 isUserDefined,
                 annotations,
                 attributes: attributes)
        {
            Expression = value;
        }

        public ValuedDeclaration(ComField field, Declaration parent, QualifiedModuleName module)
            : base(field, parent, module)
        {
            Expression = field.DefaultValue is string stringDefault 
                ? stringDefault.ToVbExpression(false) 
                : field.DefaultValue.ToString();
        }

        public ValuedDeclaration(ComEnumerationMember member, Declaration parent, QualifiedModuleName module)
            : base(member, parent, module)
        {
            Expression = member.Value.ToString();
        }

        public string Expression { get; }
    }
}
