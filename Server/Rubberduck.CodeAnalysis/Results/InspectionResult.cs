using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using System.Collections.Generic;

namespace Rubberduck.CodeAnalysis.Abstract
{
    public abstract class InspectionResult : IInspectionResult
    {
        protected InspectionResult(
            IInspection inspection, 
            string description, 
            VBABaseParserRuleContext context,
            object target,
            QualifiedMemberName member,
            QualifiedDocumentOffset offset,
            ICollection<string> disabledQuickFixes = null)
        {
            Inspection = inspection;
            Description = description;
            QualifiedMemberName = member;
            QualifiedOffset = offset;
            Context = context;
            Target = target;
            QualifiedMemberName = member;
            DisabledQuickFixes = disabledQuickFixes ?? new List<string>();
        }

        public IInspection Inspection { get; }
        public string Description { get; }
        public QualifiedDocumentOffset QualifiedOffset { get; }
        public QualifiedMemberName? QualifiedMemberName { get; }
        public VBABaseParserRuleContext Context { get; }
        public object Target { get; }
        public ICollection<string> DisabledQuickFixes { get; }
    }
}
