using Antlr4.Runtime.Tree;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using resx = Rubberduck.Resources.Inspections;

namespace Rubberduck.CodeAnalysis.Abstract
{
    public interface IInspectionConfig
    {
        string Name { get; }
        CodeInspectionSeverity CodeInspectionSeverity { get; set; }
    }

    public interface IInspectionConfig<TConfig> : IInspectionConfig
    {
        TConfig Configuration { get; set; }
    }

    public abstract class InspectionBase : IInspection
    {
        protected InspectionBase()
        {
            Name = GetType().Name;
        }

        public string Name { get; }

        /// <summary>
        /// Gets the type of inspection; used for regrouping inspections.
        /// </summary>
        public abstract CodeInspectionType InspectionType { get; }

        /// <summary>
        /// Gets a localized string representing a short name/description for the inspection.
        /// </summary>
        public virtual string Description => resx.InspectionNames.ResourceManager.GetString($"{Name}", CultureInfo.CurrentUICulture);
        
        public IEnumerable<IInspectionResult> GetInspectionResults(CancellationToken token)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = GetInspectionResults()/*.Where(ir => !ir.IsIgnoringInspectionResult())*/.ToList();
            stopwatch.Stop();
            return result;
        }

        public IEnumerable<IInspectionResult> GetInspectionResults(QualifiedModuleName module, CancellationToken token)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = GetInspectionResults(module)/*.Where(ir => !ir.IsIgnoringInspectionResult())*/.ToList();
            stopwatch.Stop();
            return result;
        }

        protected abstract IEnumerable<IInspectionResult> GetInspectionResults();
        protected abstract IEnumerable<IInspectionResult> GetInspectionResults(QualifiedModuleName module);
    }

    public abstract class ParseTreeInspectionBase<TContext> : InspectionBase, IParseTreeInspection
    where TContext : VBABaseParserRuleContext
    {
        public IInspectionListener Listener => ContextListener;

        protected abstract IInspectionListener<TContext> ContextListener { get; }

        protected abstract string ResultDescription(QualifiedContext<TContext> context);

        protected virtual bool IsResultContext(QualifiedContext<TContext> context) => true;

        protected override IEnumerable<IInspectionResult> GetInspectionResults()
        {
            return GetInspectionResults(ContextListener.Contexts());
        }

        protected override IEnumerable<IInspectionResult> GetInspectionResults(QualifiedModuleName module)
        {
            return GetInspectionResults(ContextListener.Contexts(module));
        }

        private IEnumerable<IInspectionResult> GetInspectionResults(IEnumerable<QualifiedContext<TContext>> contexts)
        {
            var objectionableContexts = contexts.Where(context => IsResultContext(context));
            return objectionableContexts.Select(InspectionResult).ToList();
        }

        protected virtual IInspectionResult InspectionResult(QualifiedContext<TContext> context)
        {
            return new QualifiedContextInspectionResult<TContext>(
                this,
                ResultDescription(context),
                context,
                DisabledQuickFixes(context));
        }

        protected virtual ICollection<string> DisabledQuickFixes(QualifiedContext<TContext> context) => new List<string>();
    }

    public interface IInspectionResult
    {
        string Description { get; }
        QualifiedDocumentOffset QualifiedOffset { get; }
        QualifiedMemberName? QualifiedMemberName { get; }
        IInspection Inspection { get; }
        object Target { get; } // TODO Declaration model
        VBABaseParserRuleContext Context { get; }
        ICollection<string> DisabledQuickFixes { get; }
    }

    public interface IWithProperties<T>
    {
        T Properties { get; }
    }
    public interface IInspection
    {
        string Name { get; }

        /// <summary>
        /// Runs code inspection and returns inspection results.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns inspection results, if any.</returns>
        IEnumerable<IInspectionResult> GetInspectionResults(CancellationToken token);

        /// <summary>
        /// Runs code inspection for a module and returns inspection results.
        /// </summary>
        /// <param name="module">The module for which to get inspection results</param>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<IInspectionResult> GetInspectionResults(QualifiedModuleName module, CancellationToken token);
    }

    public interface IParseTreeInspection : IInspection
    {
        IInspectionListener Listener { get; }
    }

    public interface IInspectionListener : IParseTreeListener
    {
        void ClearContexts();
        void ClearContexts(QualifiedModuleName module);
        QualifiedModuleName CurrentModuleName { get; set; }
    }

    public interface IInspectionListener<TContext> : IInspectionListener
        where TContext : VBABaseParserRuleContext
    {
        IReadOnlyList<QualifiedContext<TContext>> Contexts();
        IReadOnlyList<QualifiedContext<TContext>> Contexts(QualifiedModuleName module);
    }
}
