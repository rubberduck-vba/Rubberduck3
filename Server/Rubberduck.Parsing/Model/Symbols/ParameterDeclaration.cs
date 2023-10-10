using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class ParameterDeclaration : Declaration
    {
        /// <summary>
        /// Creates a new built-in parameter declaration.
        /// </summary>
        public ParameterDeclaration(QualifiedMemberName qualifiedName,
            Declaration parentDeclaration,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            bool isOptional,
            bool isByRef,
            bool isArray = false,
            bool isParamArray = false,
            string defaultValue = "")
            : base(
                  qualifiedName,
                  parentDeclaration,
                  parentDeclaration,
                  asTypeName,
                  typeHint,
                  false,
                  false,
                  Accessibility.Implicit,
                  DeclarationType.Parameter,
                  DocumentOffset.Invalid,
                  isArray,
                  asTypeContext?.AS() != null,
                  false)
        {
            IsOptional = isOptional;
            IsByRef = isByRef;
            IsImplicitByRef = false;
            IsParamArray = isParamArray;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Creates a new user declaration for a parameter.
        /// </summary>
        public ParameterDeclaration(QualifiedMemberName qualifiedName,
            Declaration parentDeclaration,
            VBABaseParserRuleContext context,
            DocumentOffset offset,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            bool isOptional,
            bool isByRef,
            bool isArray = false,
            bool isParamArray = false,
            bool isUserDefined = true)
            : base(
                  qualifiedName,
                  parentDeclaration,
                  parentDeclaration,
                  asTypeName,
                  typeHint,
                  false,
                  false,
                  Accessibility.Implicit,
                  DeclarationType.Parameter,
                  offset,
                  isArray,
                  asTypeContext?.AS() != null,
                  isUserDefined)
        {
            var argContext = context as VBAParser.ArgContext;
            IsOptional = isOptional;
            IsByRef = isByRef;
            IsImplicitByRef = isByRef && argContext?.BYREF() == null;
            IsParamArray = isParamArray;

            if (!isOptional || argContext?.argDefaultValue() == null)
            {
                return;
            }

            DefaultValue = argContext.argDefaultValue().expression()?.GetText() ?? string.Empty;
        }

        public ParameterDeclaration(ComParameter parameter, Declaration parent, QualifiedModuleName module)
            : this(
                module.QualifyMemberName(parameter.Name),
                parent,
                parameter.TypeName,
                null,
                null,
                parameter.IsOptional,
                parameter.IsByRef,
                parameter.IsArray,
                parameter.IsParamArray)
        {
            if (!string.IsNullOrEmpty(parameter.DefaultAsEnum))
            {
                DefaultValue = parameter.DefaultAsEnum;
            }
            else if (parameter.HasDefaultValue)
            {
                DefaultValue = parameter.DefaultValue is string defaultValue ? defaultValue.ToVbExpression(false) : parameter.DefaultValue.ToString();
            }
        }

        public bool IsOptional { get; }
        public bool IsByRef { get; }
        public bool IsImplicitByRef { get; }
        public bool IsParamArray { get; set; }
        public string DefaultValue { get; set; } = string.Empty;

        private ConcurrentDictionary<ArgumentReference, int> _argumentReferences = new ConcurrentDictionary<ArgumentReference, int>();
        public IEnumerable<ArgumentReference> ArgumentReferences => _argumentReferences.Keys;

        public void AddArgumentReference(
            QualifiedModuleName module,
            Declaration scope,
            Declaration parent,
            DocumentOffset offset,
            VBABaseParserRuleContext argumentContext,
            VBAParser.ArgumentListContext argumentListContext,
            ArgumentListArgumentType argumentType,
            int argumentPosition,
            string identifier,
            IEnumerable<IParseTreeAnnotation> annotations)
        {
            var newReference = new ArgumentReference(
                module,
                scope,
                parent,
                identifier,
                offset,
                argumentContext,
                argumentListContext,
                argumentType,
                argumentPosition,
                this,
                annotations);
            _argumentReferences.AddOrUpdate(newReference, 1, (key, value) => 1);
        }

        public override void ClearReferences()
        {
            _argumentReferences = new ConcurrentDictionary<ArgumentReference, int>();
            base.ClearReferences();
        }

        public override void RemoveReferencesFrom(IReadOnlyCollection<QualifiedModuleName> modulesByWhichToRemoveReferences)
        {
            _argumentReferences = new ConcurrentDictionary<ArgumentReference, int>(_argumentReferences.Where(reference => !modulesByWhichToRemoveReferences.Contains(reference.Key.QualifiedModuleName)));
            base.RemoveReferencesFrom(modulesByWhichToRemoveReferences);
        }
    }
}
