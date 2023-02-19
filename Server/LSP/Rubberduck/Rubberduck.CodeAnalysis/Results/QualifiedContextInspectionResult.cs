using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using System.Collections.Generic;

namespace Rubberduck.CodeAnalysis.Abstract
{
    public class QualifiedContextInspectionResult<TContext> : InspectionResult
        where TContext : VBABaseParserRuleContext
    {
        public QualifiedContextInspectionResult(
            IInspection inspection,
            string description,
            QualifiedContext<TContext> context,
            ICollection<string> disabledQuickFixes = null) :
            base(inspection,
                 description,
                 context.Context,
                 null,
                 context.MemberName,
                 context.Offset,
                 disabledQuickFixes)
        { }
    }

    public class QualifiedContextInspectionResult<TContext, TProperties> : QualifiedContextInspectionResult<TContext>, IWithProperties<TProperties>
        where TContext : VBABaseParserRuleContext
        where TProperties : class
    {
        public QualifiedContextInspectionResult(
            IInspection inspection,
            string description,
            QualifiedContext<TContext> context,
            TProperties properties,
            ICollection<string> disabledQuickFixes = null) :
            base(
                inspection,
                description,
                context,
                disabledQuickFixes)
        {
            Properties = properties;
        }

        public TProperties Properties { get; }
    }
}
