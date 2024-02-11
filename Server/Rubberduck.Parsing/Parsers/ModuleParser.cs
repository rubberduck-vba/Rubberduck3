using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Parsing.Parsers;

//public abstract class ModuleParser<TContent> : IModuleParser
//{
//    protected ModuleParser(IParser<TContent> parser, IAnnotationFactory annotationFactory, ILogger<ModuleParser<TContent>> logger)
//    {
//        Parser = parser;
//        AnnotationFactory = annotationFactory;
//        Logger = logger;
//    }

//    protected IParser<TContent> Parser { get; }
//    protected IAnnotationFactory AnnotationFactory { get; }
//    protected ILogger<ModuleParser<TContent>> Logger;

//    protected abstract ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId);

//    protected (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines, int contentHash) ParseInternal(CodeKind codeKind, ISourceCodeProvider<TContent> provider, QualifiedModuleName module, CancellationToken token)
//    {
//        token.ThrowIfCancellationRequested();
//        var code = provider.SourceCode(module);

//        token.ThrowIfCancellationRequested();
//        var contentHash = provider.GetContentHash(module);

//        token.ThrowIfCancellationRequested();
//        var (tree, tokenStream, logicalLines) = Parser.Parse(module.ComponentName, module.ProjectId, code, token, codeKind);

//        return (tree, tokenStream, logicalLines, contentHash);
//    }

//    public ModuleParseResults Parse(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter = null)
//    {
//        var taskId = Guid.NewGuid();
//        try
//        {
//            return ParseInternal(module, cancellationToken, rewriter, taskId);
//        }
//        catch (COMException exception)
//        {
//            Logger.LogError(exception, $"COM Exception thrown in thread {Environment.CurrentManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
//            throw;
//        }
//        catch (PreprocessorSyntaxErrorException syntaxErrorException)
//        {
//            Logger.LogError($"Syntax error while preprocessing; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
//            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
//            throw;
//        }
//        catch (ParsePassSyntaxErrorException syntaxErrorException)
//        {
//            Logger.LogError($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in the {syntaxErrorException.CodeKind} version of module {module.ComponentName}.");
//            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
//            throw;
//        }
//        catch (SyntaxErrorException syntaxErrorException)
//        {
//            Logger.LogError($"Syntax error; offending token '{syntaxErrorException.OffendingSymbol.Text}' at line {syntaxErrorException.LineNumber}, column {syntaxErrorException.Position} in module {module.ComponentName}.");
//            Logger.LogDebug(syntaxErrorException, $"SyntaxErrorException thrown in thread {Environment.CurrentManagedThreadId}, ParseTaskID {taskId}.");
//            throw;
//        }
//        catch (OperationCanceledException)
//        {
//            //We rethrow this, so that the calling code knows that the operation actually has been cancelled.
//            //No need to log it.
//            throw;
//        }
//        catch (Exception exception)
//        {
//            Logger.LogError(exception, $" Unexpected exception thrown in thread {Environment.CurrentManagedThreadId} while parsing module {module.ComponentName}, ParseTaskID {taskId}.");
//            throw;
//        }
//    }

//}
