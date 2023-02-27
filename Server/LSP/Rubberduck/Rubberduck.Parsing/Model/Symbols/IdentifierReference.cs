using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using Rubberduck.Parsing.Annotations;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model.Symbols
{
    [DebuggerDisplay("({IdentifierName}) IsAss:{IsAssignment} | {Selection} ")]
    public class IdentifierReference : IEquatable<IdentifierReference>
    {
        internal IdentifierReference(
            QualifiedModuleName qualifiedName, 
            Declaration parentScopingDeclaration, 
            Declaration parentNonScopingDeclaration, 
            string identifierName,
            DocumentOffset offset,
            VBABaseParserRuleContext context, 
            Declaration declaration, 
            bool isAssignmentTarget = false,
            bool hasExplicitLetStatement = false, 
            IEnumerable<IParseTreeAnnotation> annotations = null,
            bool isSetAssigned = false,
            bool isIndexedDefaultMemberAccess = false,
            bool isNonIndexedDefaultMemberAccess = false,
            int defaultMemberRecursionDepth = 0,
            bool isArrayAccess = false,
            bool isProcedureCoercion = false,
            bool isInnerRecursiveDefaultMemberAccess = false,
            IdentifierReference qualifyingReference = null,
            bool isReDim = false)
        {
            ParentScoping = parentScopingDeclaration;
            ParentNonScoping = parentNonScopingDeclaration;
            IdentifierName = identifierName;
            Declaration = declaration;
            HasExplicitLetStatement = hasExplicitLetStatement;
            IsAssignment = isAssignmentTarget;
            IsSetAssignment = isSetAssigned;
            IsIndexedDefaultMemberAccess = isIndexedDefaultMemberAccess;
            IsNonIndexedDefaultMemberAccess = isNonIndexedDefaultMemberAccess;
            DefaultMemberRecursionDepth = defaultMemberRecursionDepth;
            IsArrayAccess = isArrayAccess;
            IsProcedureCoercion = isProcedureCoercion;
            Annotations = annotations ?? new List<IParseTreeAnnotation>();
            IsInnerRecursiveDefaultMemberAccess = isInnerRecursiveDefaultMemberAccess;
            QualifyingReference = qualifyingReference;
            IsReDim = isReDim;

            QualifiedOffset = new QualifiedDocumentOffset(qualifiedName, offset);
            QualifiedModuleName = QualifiedOffset.QualifiedModuleName;

            HasExplicitCallStatement = context.Parent is VBAParser.CallStmtContext callStmt && callStmt.CALL() != null;
            HasTypeHint = HasTypeHintToken(context, out var typeHintToken);
            TypeHintToken= typeHintToken;
        }

        public QualifiedDocumentOffset QualifiedOffset { get; }

        public QualifiedModuleName QualifiedModuleName { get; }

        public string IdentifierName { get; }

        /// <summary>
        /// Gets the scoping <see cref="Declaration"/> that contains this identifier reference,
        /// e.g. a module, procedure, function or property.
        /// </summary>
        public Declaration ParentScoping { get; }

        /// <summary>
        /// Gets the non-scoping <see cref="Declaration"/> that contains this identifier reference,
        /// e.g. a user-defined or enum type. Gets the <see cref="ParentScoping"/> if not applicable.
        /// </summary>
        public Declaration ParentNonScoping { get; }

        public IdentifierReference QualifyingReference { get; }

        public bool IsAssignment { get; }

        public bool IsSetAssignment { get; }

        public bool IsIndexedDefaultMemberAccess { get; }
        public bool IsNonIndexedDefaultMemberAccess { get; }
        public bool IsDefaultMemberAccess => IsIndexedDefaultMemberAccess || IsNonIndexedDefaultMemberAccess;
        public bool IsProcedureCoercion { get; }
        public bool IsInnerRecursiveDefaultMemberAccess { get; }
        public int DefaultMemberRecursionDepth { get; }

        public bool IsArrayAccess { get; }
        public bool IsReDim { get; }

        public Declaration Declaration { get; }

        public IEnumerable<IParseTreeAnnotation> Annotations { get; }

        public bool HasExplicitLetStatement { get; }

        public bool HasExplicitCallStatement { get; }

        public bool HasTypeHint { get; }
        public string TypeHintToken { get; }

        private static bool HasTypeHintToken(VBABaseParserRuleContext context, out string token)
        {
            if (context is null)
            {
                token = null;
                return false;
            }
            var method = context.Parent.GetType().GetMethods().FirstOrDefault(m => m.Name == "typeHint");
            if (method == null)
            {
                token = null;
                return false;
            }

            var hint = context.Parent is VBAParser.TypedIdentifierContext typedIdentifierContext
                ? typedIdentifierContext.typeHint()
                : null;

            token = hint?.GetText();
            return hint != null;
        }

        public bool IsSelected(QualifiedDocumentOffset qualifiedOffset)
        {
            return QualifiedModuleName.Equals(qualifiedOffset.QualifiedModuleName) && 
                QualifiedOffset.Offset.Start <= qualifiedOffset.Offset.Start &&
                QualifiedOffset.Offset.End >= qualifiedOffset.Offset.End;
        }

        public bool Equals(IdentifierReference other)
        {
            return other != null
                && other.QualifiedModuleName.Equals(QualifiedModuleName)
                && other.QualifiedOffset.Equals(QualifiedOffset)
                && (other.Declaration != null && other.Declaration.Equals(Declaration)
                    || other.Declaration == null && Declaration == null);
        }

        public override bool Equals(object obj) => Equals(obj as IdentifierReference);

        public override int GetHashCode() => HashCode.Combine(QualifiedModuleName, QualifiedOffset, Declaration);
    }
}
