using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Model;
using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Parsing.Parsers;

public abstract class ModuleParser<TContent> : IModuleParser
{
    protected ModuleParser(IParser<TContent> parser, IAnnotationFactory annotationFactory, ILogger<ModuleParser<TContent>> logger)
    {
        Parser = parser;
        AnnotationFactory = annotationFactory;
        Logger = logger;
    }

    protected IParser<TContent> Parser { get; }
    protected IAnnotationFactory AnnotationFactory { get; }
    protected ILogger<ModuleParser<TContent>> Logger;

    protected abstract ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId);

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
            Logger.LogError(exception, $"COM Exception thrown in thread {Environment.CurrentManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
            throw;
        }
        catch (PreprocessorSyntaxErrorException syntaxErrorException)
        {
            Logger.LogError($"Syntax error while preprocessing; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
            throw;
        }
        catch (ParsePassSyntaxErrorException syntaxErrorException)
        {
            Logger.LogError($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
            throw;
        }
        catch (SyntaxErrorException syntaxErrorException)
        {
            Logger.LogError($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in module {module.ComponentName}.");
            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
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
            Logger.LogError(exception, $" Unexpected exception thrown in thread {Environment.CurrentManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
            throw;
        }
    }

}

public class VbeModuleParser : ModuleParser<string>
{
    private readonly IDictionary<CodeKind, ISourceCodeProvider<string>> _sourceCodeProviders;

    public VbeModuleParser(ISourceCodeProvider<string> editor, IStringParser parser, IAnnotationFactory annotationFactory, ILogger<VbeModuleParser> logger)
        : this(new Dictionary<CodeKind, ISourceCodeProvider<string>> { [CodeKind.RubberduckEditorModule] = editor }, logger, parser, annotationFactory)
    {
    }
    public VbeModuleParser(IDictionary<CodeKind, ISourceCodeProvider<string>> sourceCodeProviders, ILogger<VbeModuleParser> logger, IStringParser parser, IAnnotationFactory annotationFactory)
        : base(parser, annotationFactory, logger)
    {
        _sourceCodeProviders = sourceCodeProviders;
    }

    protected override ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId)
    {
        Logger.LogTrace($"Starting ParseTaskID {taskId} on thread {Environment.CurrentManagedThreadId}.");

        cancellationToken.ThrowIfCancellationRequested();

        Logger.LogTrace($"ParseTaskID {taskId} begins attributes pass.");
        var (attributesParseTree, attributesTokenStream, logicalLines, contentHash) = AttributesPassResults(module, cancellationToken);

        Logger.LogTrace($"ParseTaskID {taskId} finished attributes pass.");
        cancellationToken.ThrowIfCancellationRequested();

        Logger.LogTrace($"ParseTaskID {taskId} begins extracting comments, annotations, and attributes.");
        var type = module.ComponentType == ComponentType.StandardModule
            ? DeclarationType.ProceduralModule
            : DeclarationType.ClassModule;
        //var attributesListener = new AttributeListener((module.ComponentName, type));
        //var (comments, annotations) = TraverseForCommentsAndAnnotations(module, attributesParseTree, attributesListener);

        Logger.LogTrace($"ParseTaskID {taskId} finished extracting comments, annotations, and attributes.");
        cancellationToken.ThrowIfCancellationRequested();

        return new ModuleParseResults(
            new Dictionary<CodeKind, (IParseTree, ITokenStream)> 
            { 
                [CodeKind.AttributesCode] = (attributesParseTree, attributesTokenStream),
                [CodeKind.RubberduckEditorModule] = (attributesParseTree, attributesTokenStream),
            },
            contentHash,
            //null, //comments,
            null, //annotations,
            logicalLines,
            null, //attributesListener.Attributes,
            null //attributesListener.MembersAllowingAttributes
        );
    }

    private (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines, int contentHash) AttributesPassResults(QualifiedModuleName module, CancellationToken token)
    {
        return ParseInternal(CodeKind.AttributesCode, _sourceCodeProviders[CodeKind.AttributesCode], module, token);
    }
}