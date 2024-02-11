using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Model;
using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Parsing.Parsers;

//public class VbeModuleParser : ModuleParser<string>
//{
//    private readonly IDictionary<CodeKind, ISourceCodeProvider<string>> _sourceCodeProviders;

//    public VbeModuleParser(ISourceCodeProvider<string> editor, IStringParser parser, IAnnotationFactory annotationFactory, ILogger<VbeModuleParser> logger)
//        : this(new Dictionary<CodeKind, ISourceCodeProvider<string>> { [CodeKind.RubberduckEditorModule] = editor }, logger, parser, annotationFactory)
//    {
//    }
//    public VbeModuleParser(IDictionary<CodeKind, ISourceCodeProvider<string>> sourceCodeProviders, ILogger<VbeModuleParser> logger, IStringParser parser, IAnnotationFactory annotationFactory)
//        : base(parser, annotationFactory, logger)
//    {
//        _sourceCodeProviders = sourceCodeProviders;
//    }

//    protected override ModuleParseResults ParseInternal(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter, Guid taskId)
//    {
//        Logger.LogTrace($"Starting ParseTaskID {taskId} on thread {Environment.CurrentManagedThreadId}.");

//        cancellationToken.ThrowIfCancellationRequested();

//        Logger.LogTrace($"ParseTaskID {taskId} begins attributes pass.");
//        var (attributesParseTree, attributesTokenStream, logicalLines, contentHash) = AttributesPassResults(module, cancellationToken);

//        Logger.LogTrace($"ParseTaskID {taskId} finished attributes pass.");
//        cancellationToken.ThrowIfCancellationRequested();

//        Logger.LogTrace($"ParseTaskID {taskId} begins extracting comments, annotations, and attributes.");
//        var type = module.ComponentType == ComponentType.StandardModule
//            ? DeclarationType.ProceduralModule
//            : DeclarationType.ClassModule;
//        //var attributesListener = new AttributeListener((module.ComponentName, type));
//        //var (comments, annotations) = TraverseForCommentsAndAnnotations(module, attributesParseTree, attributesListener);

//        Logger.LogTrace($"ParseTaskID {taskId} finished extracting comments, annotations, and attributes.");
//        cancellationToken.ThrowIfCancellationRequested();

//        return new ModuleParseResults(
//            new Dictionary<CodeKind, (IParseTree, ITokenStream)> 
//            { 
//                [CodeKind.AttributesCode] = (attributesParseTree, attributesTokenStream),
//                [CodeKind.RubberduckEditorModule] = (attributesParseTree, attributesTokenStream),
//            },
//            contentHash,
//            //null, //comments,
//            null, //annotations,
//            logicalLines,
//            null, //attributesListener.Attributes,
//            null //attributesListener.MembersAllowingAttributes
//        );
//    }

//    private (IParseTree tree, ITokenStream tokenStream, LogicalLineStore logicalLines, int contentHash) AttributesPassResults(QualifiedModuleName module, CancellationToken token)
//    {
//        return ParseInternal(CodeKind.AttributesCode, _sourceCodeProviders[CodeKind.AttributesCode], module, token);
//    }
//}