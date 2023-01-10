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

        protected LogicalLineStore LogicalLines(string code)
        {
            var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var logicalLineEnds = lines
                .Select((line, index) => (line, index))
                .Where(tpl => !tpl.line.TrimEnd().EndsWith(" _")) //Not line-continued
                .Select(tpl => tpl.index + 1); //VBA lines are 1-based.
            return new LogicalLineStore(logicalLineEnds);
        }

        protected (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines, int contentHash) ParseInternal(CodeKind codeKind, ISourceCodeProvider<TContent> provider, QualifiedModuleName module, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var code = provider.SourceCode(module);

            token.ThrowIfCancellationRequested();
            var contentHash = provider.GetContentHash(module);

            token.ThrowIfCancellationRequested();
            var (tree, tokenStream, logicalLines) = Parser.Parse(module.ComponentName, module.ProjectId, code, token, codeKind);

            return (tree, tokenStream, logicalLines, contentHash);
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

        protected virtual (IEnumerable<CommentNode> Comments, IEnumerable<IParseTreeAnnotation> Annotations) TraverseForCommentsAndAnnotations(QualifiedModuleName module, IParseTree tree, params VBAParserBaseListener[] listeners)
        {
            var commentListener = new CommentListener();
            var annotationListener = new AnnotationListener(AnnotationFactory, module);
            var combinedListener = new CombinedParseTreeListener(new IParseTreeListener[] { commentListener, annotationListener }.Union(listeners));
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

        public EditorModuleParser(IParser<TextReader> parser, IAnnotationFactory annotationFactory, ISourceCodeProvider<TextReader> provider) 
            : base(parser, annotationFactory)
        {
            _provider = provider;
        }

        protected override ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (tree, tokenStream, logicalLines, contentHash) = ParseInternal(CodeKind.RubberduckEditorModule, _provider, module, cancellationToken);

            var type = module.ComponentType == ComponentType.StandardModule
                ? DeclarationType.ProceduralModule
                : DeclarationType.ClassModule;
            var attributesListener = new AttributeListener((module.ComponentName, type));

            var (comments, annotations) = TraverseForCommentsAndAnnotations(module, tree, attributesListener);

            return new ModuleParseResults(
                new Dictionary<CodeKind, (IParseTree, ITokenStream)> 
                {
                    [CodeKind.AttributesCode] = (tree, tokenStream),
                    [CodeKind.RubberduckEditorModule] = (tree, tokenStream),
                },
                contentHash,
                comments, 
                annotations, 
                logicalLines, 
                attributesListener.Attributes, 
                attributesListener.MembersAllowingAttributes);
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

            Logger.Trace($"ParseTaskID {taskId} begins attributes pass.");
            var (attributesParseTree, attributesTokenStream, logicalLines, contentHash) = AttributesPassResults(module, cancellationToken);

            Logger.Trace($"ParseTaskID {taskId} finished attributes pass.");
            cancellationToken.ThrowIfCancellationRequested();

            Logger.Trace($"ParseTaskID {taskId} begins extracting comments, annotations, and attributes.");
            var type = module.ComponentType == ComponentType.StandardModule
                ? DeclarationType.ProceduralModule
                : DeclarationType.ClassModule;
            var attributesListener = new AttributeListener((module.ComponentName, type));
            var (comments, annotations) = TraverseForCommentsAndAnnotations(module, attributesParseTree, attributesListener);

            Logger.Trace($"ParseTaskID {taskId} finished extracting comments, annotations, and attributes.");
            cancellationToken.ThrowIfCancellationRequested();

            return new ModuleParseResults(
                new Dictionary<CodeKind, (IParseTree, ITokenStream)> 
                { 
                    [CodeKind.AttributesCode] = (attributesParseTree, attributesTokenStream),
                    [CodeKind.RubberduckEditorModule] = (attributesParseTree, attributesTokenStream),
                },
                contentHash,
                comments,
                annotations,
                logicalLines,
                attributesListener.Attributes,
                attributesListener.MembersAllowingAttributes
            );
        }

        private (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines, int contentHash) AttributesPassResults(QualifiedModuleName module, CancellationToken token)
        {
            return ParseInternal(CodeKind.AttributesCode, _sourceCodeProviders[CodeKind.AttributesCode], module, token);
        }

        //private (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines) CodePanePassResults(QualifiedModuleName module, CancellationToken token, TokenStreamRewriter rewriter = null)
        //{
        //    var result = ParseInternal(CodeKind.CodePaneCode, _sourceCodeProviders[CodeKind.CodePaneCode], module, token);
            
        //    token.ThrowIfCancellationRequested();
        //    var code = rewriter?.GetText() ?? _sourceCodeProviders[CodeKind.CodePaneCode].SourceCode(module);

        //    token.ThrowIfCancellationRequested();
        //    var logicalLines = LogicalLines(code);

        //    return (result.tree, result.tokenStream, logicalLines);
        //}
    }
}