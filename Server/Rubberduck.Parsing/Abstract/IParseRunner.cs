using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.VBA.Parsing
{
    public interface IParseRunner
    {
        Task ParseModules(IReadOnlyCollection<QualifiedModuleName> modulesToParse, CancellationToken token);
    }

    public abstract class ParseRunnerBase : IParseRunner
    {
        //protected readonly IDeclarationServices _declarationServices;
        private readonly IModuleParser _parser;

        protected IParserStateManager StateManager { get; }

        protected ParseRunnerBase(
            //IDeclarationServices declarationServices,
            IParserStateManager parserStateManager,
            IModuleParser parser)
        {
            //_declarationServices = declarationServices ?? throw new ArgumentNullException(nameof(declarationServices));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            StateManager = parserStateManager ?? throw new ArgumentNullException(nameof(parserStateManager));
        }

        public async Task ParseModules(IReadOnlyCollection<QualifiedModuleName> modules, CancellationToken token)
        {
            //var parsingStageTimer = ParsingStageTimer.StartNew();

            var parseResults = ModuleParseResults(modules, token);
            Save(parseResults, token);

            //parsingStageTimer.Stop();
            //parsingStageTimer.Log("Parsed user modules in {0}ms.");
        }

        protected abstract IReadOnlyCollection<(QualifiedModuleName module, ModuleParseResults results)> ModuleParseResults(IReadOnlyCollection<QualifiedModuleName> modules, CancellationToken token);

        protected ModuleParseResults ModuleParseResults(QualifiedModuleName module, CancellationToken token)
        {
            return _parser.Parse(module, token);

            //_state.ClearStateCache(module);
            //try
            //{
            //    return _parser.Parse(module, token);
            //}
            //catch (SyntaxErrorException syntaxErrorException)
            //{
            //    //In contrast to the situation in the success scenario, the overall parser state is reevaluated immediately.
            //    //This sets the state directly on the state because it is the sole instance where we have to pass the SyntaxErrorException.
            //    _state.SetModuleState(module, ParserState.Error, token, syntaxErrorException);
            //    return default;
            //}
            //catch (Exception exception)
            //{
            //    StateManager.SetStatusAndFireStateChanged(this, ParserState.Error, token);
            //    throw;
            //}
        }

        private void Save(IReadOnlyCollection<(QualifiedModuleName module, ModuleParseResults results)> parseResults, CancellationToken token)
        {
            foreach (var (module, result) in parseResults)
            {
                if (result[CodeKind.RubberduckEditorModule].tree is null)
                {
                    continue;
                }

                Save(module, result, token);
            }
        }

        private void Save(QualifiedModuleName module, ModuleParseResults results, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            StateManager.SetModuleParserState(module, ParserState.Parsed, token);
            
            //This has to come first because it creates the module state if not present.
            //_state.AddModuleStateIfNotPresent(module);

            //_state.SaveContentHash(module);
            //_state.AddParseTree(module, results.ParseTree[CodeKind.CodePaneCode]);
            //_state.AddParseTree(module, results.ParseTree[CodeKind.AttributesCode], CodeKind.AttributesCode);
            //_state.SetModuleComments(module, results.Comments);
            //_state.SetModuleAnnotations(module, results.Annotations);
            //_state.SetModuleLogicalLines(module, results.LogicalLines);
            //_state.SetModuleAttributes(module, results.Attributes);
            //_state.SetMembersAllowingAttributes(module, results.MembersAllowingAttributes);
            //_state.SetCodePaneTokenStream(module, results.CodePaneTokenStream);
            //_state.SetAttributesTokenStream(module, results.AttributesTokenStream);

            // This really needs to go last
            //It does not reevaluate the overall parser state to avoid concurrent evaluation of all module states and for performance reasons.
            //The evaluation has to be triggered manually in the calling procedure.
            //StateManager.SetModuleState(module, ParserState.Parsed, token, false); //Note that this is ok because locks allow re-entry.
        }
    }
}
