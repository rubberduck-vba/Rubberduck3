using System;
using System.Runtime.InteropServices;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SourceCodeHandling;
using NLog;
using System.Collections.Generic;
using Rubberduck.Parsing.Model.Symbols;
using Rubberduck.Parsing.Listeners;
using Rubberduck.Parsing.Grammar;
using System.Linq;
using Rubberduck.VBEditor.SafeComWrappers;
using System.Dynamic;
using System.IO;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Parsers
{
    public abstract class ModuleParser<TContent> : IModuleParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected ModuleParser(IParser<TContent> parser, IAnnotationFactory annotationFactory)
        {
            Parser = parser;
            AnnotationFactory = annotationFactory;
        }

        protected IParser<TContent> Parser { get; }
        protected IAnnotationFactory AnnotationFactory { get; }

        protected abstract ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId);

        protected (IParseTree tree, ITokenStream tokenStream) ParseInternal(CodeKind codeKind, ISourceCodeProvider<TContent> provider, QualifiedModuleName module, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var code = provider.SourceCode(module);

            token.ThrowIfCancellationRequested();
            var results = Parser.Parse(module.ComponentName, module.ProjectId, code, token, codeKind);

            return results;
        }

        public ModuleParseResults Parse(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter = null)
        {
            var taskId = Guid.NewGuid();
            try
            {
                return ParseInternal(module, cancellationToken, rewriter, taskId);
            }
            catch (COMException exception)
            {
                Logger.Error(exception, $"COM Exception thrown in thread {Thread.CurrentThread.ManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
                throw;
            }
            catch (PreprocessorSyntaxErrorException syntaxErrorException)
            {
                Logger.Error($"Syntax error while preprocessing; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
                Logger.Debug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Thread.CurrentThread.ManagedThreadId}, ParseTaskID {taskId}.");
                throw;
            }
            catch (ParsePassSyntaxErrorException syntaxErrorException)
            {
                Logger.Error($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
                Logger.Debug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Thread.CurrentThread.ManagedThreadId}, ParseTaskID {taskId}.");
                throw;
            }
            catch (SyntaxErrorException syntaxErrorException)
            {
                Logger.Error($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in module {module.ComponentName}.");
                Logger.Debug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Thread.CurrentThread.ManagedThreadId}, ParseTaskID {taskId}.");
                throw;
            }
            catch (OperationCanceledException)
            {
                //We rethrow this, so that the calling code knows that the operation actually has been cancelled.
                //No need to log it.
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $" Unexpected exception thrown in thread {Thread.CurrentThread.ManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
                throw;
            }
        }

        protected virtual (IEnumerable<CommentNode> Comments, IEnumerable<IParseTreeAnnotation> Annotations) TraverseForCommentsAndAnnotations(QualifiedModuleName module, IParseTree tree)
        {
            var commentListener = new CommentListener();
            var annotationListener = new AnnotationListener(AnnotationFactory, module);
            var combinedListener = new CombinedParseTreeListener(new IParseTreeListener[] { commentListener, annotationListener });
            ParseTreeWalker.Default.Walk(combinedListener, tree);

            var comments = QualifyAndUnionComments(module, commentListener.Comments, commentListener.RemComments);
            var annotations = annotationListener.Annotations;
            return (comments, annotations);
        }

        protected IEnumerable<CommentNode> QualifyAndUnionComments(QualifiedModuleName qualifiedName, IEnumerable<VBAParser.CommentContext> comments, IEnumerable<VBAParser.RemCommentContext> remComments)
        {
            var commentNodes = comments.Select(comment => new CommentNode(comment.GetComment(), Tokens.CommentMarker, new QualifiedDocumentOffset(qualifiedName, comment.Offset)));
            var remCommentNodes = remComments.Select(comment => new CommentNode(comment.GetComment(), Tokens.Rem, new QualifiedDocumentOffset(qualifiedName, comment.Offset)));
            var allCommentNodes = commentNodes.Union(remCommentNodes);
            return allCommentNodes;
        }

        protected (IDictionary<(string scopeIdentifier, DeclarationType scopeType), Attributes> attributes,
            IDictionary<(string scopeIdentifier, DeclarationType scopeType), ParserRuleContext> membersAllowingAttributes)
            TraverseForAttributes(QualifiedModuleName module, IParseTree tree)
        {
            var type = module.ComponentType == ComponentType.StandardModule
                ? DeclarationType.ProceduralModule
                : DeclarationType.ClassModule;
            var attributesListener = new AttributeListener((module.ComponentName, type));
            ParseTreeWalker.Default.Walk(attributesListener, tree);
            return (attributesListener.Attributes, attributesListener.MembersAllowingAttributes);
        }
    }

    public class EditorModuleParser : ModuleParser<TextReader>
    {
        private readonly ISourceCodeProvider<TextReader> _provider;

        public EditorModuleParser(IParser<TextReader> parser, IAnnotationFactory annotationFactory) 
            : base(parser, annotationFactory)
        {
        }

        protected override ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId)
        {
            throw new NotImplementedException();
        }
    }

    public class VbeModuleParser : ModuleParser<string>
    {
        private readonly IDictionary<CodeKind, ISourceCodeProvider<string>> _sourceCodeProviders;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public VbeModuleParser(ISourceCodeProvider<string> editor, IStringParser parser, IAnnotationFactory annotationFactory)
            : this(new Dictionary<CodeKind, ISourceCodeProvider<string>> { [CodeKind.RubberduckEditorModule] = editor }, parser, annotationFactory)
        {
        }
        public VbeModuleParser(IDictionary<CodeKind, ISourceCodeProvider<string>> sourceCodeProviders, IStringParser parser, IAnnotationFactory annotationFactory)
            : base(parser, annotationFactory)
        {
            _sourceCodeProviders = sourceCodeProviders;
        }

        protected override ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId)
        {
            Logger.Trace($"Starting ParseTaskID {taskId} on thread {Thread.CurrentThread.ManagedThreadId}.");

            cancellationToken.ThrowIfCancellationRequested();

            Logger.Trace($"ParseTaskID {taskId} begins code pane pass.");
            var (codePaneParseTree, codePaneTokenStream, logicalLines) = CodePanePassResults(module, cancellationToken, rewriter);

            Logger.Trace($"ParseTaskID {taskId} finished code pane pass.");
            cancellationToken.ThrowIfCancellationRequested();

            
            // temporal coupling... comments must be acquired before we walk the parse tree for declarations
            // otherwise none of the annotations get associated to their respective Declaration
            Logger.Trace($"ParseTaskID {taskId} begins extracting comments and annotations.");
            var (comments, annotations) = TraverseForCommentsAndAnnotations(module, codePaneParseTree);

            Logger.Trace($"ParseTaskID {taskId} finished extracting comments and annotations.");
            cancellationToken.ThrowIfCancellationRequested();

            
            Logger.Trace($"ParseTaskID {taskId} begins attributes pass.");
            var (attributesParseTree, attributesTokenStream) = AttributesPassResults(module, cancellationToken);

            Logger.Trace($"ParseTaskID {taskId} finished attributes pass.");
            cancellationToken.ThrowIfCancellationRequested();

            
            Logger.Trace($"ParseTaskID {taskId} begins extracting attributes.");
            var (attributes, membersAllowingAttributes) = TraverseForAttributes(module, attributesParseTree);

            Logger.Trace($"ParseTaskID {taskId} finished extracting attributes.");
            cancellationToken.ThrowIfCancellationRequested();

            return new ModuleParseResults(
                new Dictionary<CodeKind, (IParseTree, ITokenStream)> 
                { 
                    [CodeKind.CodePaneCode] = (codePaneParseTree, codePaneTokenStream),
                    [CodeKind.AttributesCode] = (attributesParseTree, attributesTokenStream),
                },
                comments,
                annotations,
                logicalLines,
                attributes,
                membersAllowingAttributes
            );
        }

        private (IParseTree tree, ITokenStream tokenStream) AttributesPassResults(QualifiedModuleName module, CancellationToken token)
        {
            return ParseInternal(CodeKind.AttributesCode, _sourceCodeProviders[CodeKind.AttributesCode], module, token);
        }

        private (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines) CodePanePassResults(QualifiedModuleName module, CancellationToken token, TokenStreamRewriter rewriter = null)
        {
            var result = ParseInternal(CodeKind.CodePaneCode, _sourceCodeProviders[CodeKind.CodePaneCode], module, token);
            
            token.ThrowIfCancellationRequested();
            var code = rewriter?.GetText() ?? _sourceCodeProviders[CodeKind.CodePaneCode].SourceCode(module);

            token.ThrowIfCancellationRequested();
            var logicalLines = LogicalLines(code);

            return (result.tree, result.tokenStream, logicalLines);
        }

        private LogicalLineStore LogicalLines(string code)
        {
            var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var logicalLineEnds = lines
                .Select((line, index) => (line, index))
                .Where(tpl => !tpl.line.TrimEnd().EndsWith(" _")) //Not line-continued
                .Select(tpl => tpl.index + 1); //VBA lines are 1-based.
            return new LogicalLineStore(logicalLineEnds);
        }
    }
}