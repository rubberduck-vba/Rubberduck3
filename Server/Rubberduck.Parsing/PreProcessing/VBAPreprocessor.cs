using Antlr4.Runtime;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Expressions;
using Rubberduck.Parsing.Model;
using Rubberduck.Parsing.Parsers;

namespace Rubberduck.Parsing.PreProcessing;

public sealed class VBAPreprocessor : ITokenStreamPreprocessor
{
    private readonly ITokenStreamParser _parser;
    private readonly ICompilationArgumentsProvider _compilationArgumentsProvider;

    public VBAPreprocessor(VBAPreprocessorParser preprocessorParser, ICompilationArgumentsProvider compilationArgumentsProvider)
    {
        _compilationArgumentsProvider = compilationArgumentsProvider;
        _parser = preprocessorParser;
    }

    public CommonTokenStream? PreprocessTokenStream(WorkspaceFileUri uri, CommonTokenStream tokenStream, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var tree = _parser.Parse(uri, tokenStream, token);
        token.ThrowIfCancellationRequested();

        var charStream = tokenStream.TokenSource.InputStream;
        var symbolTable = new SymbolTable<string, IValue>();
        Dictionary<string, short> userCompilationArguments = [];
        try
        {
            userCompilationArguments = _compilationArgumentsProvider.UserDefinedCompilationArguments(uri);
        }
        catch (Exception)
        {
            // could not read precompiler constants from ITypeLib
            // let's still attempt to return a token stream.
        }
        var predefinedCompilationArgument = _compilationArgumentsProvider.PredefinedCompilationConstants; 
        var evaluator = new VBAPreprocessorVisitor(symbolTable, predefinedCompilationArgument, userCompilationArguments, charStream, tokenStream);
        var expr = evaluator.Visit(tree);
        _ = expr.Evaluate(); //This does the actual preprocessing of the token stream as a side effect.
        tokenStream.Reset();
        return tokenStream;
    }
}
