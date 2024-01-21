using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Annotations;

public sealed class VBAParserAnnotationFactory : IAnnotationFactory
{
    private readonly Dictionary<string, IAnnotation> _lookup = new();
    private readonly IAnnotation _unrecognized;

    public VBAParserAnnotationFactory(IEnumerable<IAnnotation> recognizedAnnotations) 
    {
        foreach (var annotation in recognizedAnnotations)
        {
            if (annotation is NotRecognizedAnnotation)
            {
                _unrecognized = annotation;
            }
            _lookup.Add(annotation.Name.ToLowerInvariant(), annotation);
        }
    }

    public IParseTreeAnnotation Create(VBAParser.AnnotationContext context, Location location)
    {
        var annotationName = context.annotationName().GetText();
        if (!_lookup.TryGetValue(annotationName.ToLowerInvariant(), out var annotation) || annotation is null)
        {
            annotation = _unrecognized;
        }
        return new ParseTreeAnnotation(annotation, location)
        {
            AnnotationArguments = ParseTreeAnnotation.AnnotationParametersFromContext(context, annotation),
            AnnotatedLine = ParseTreeAnnotation.ResolveAnnotatedLine(context)
        };
    }
}

public class ParseTreeAnnotation : IParseTreeAnnotation
{
    public const string ANNOTATION_MARKER = "@";

    internal ParseTreeAnnotation(IAnnotation annotation, Location location)
    {
        Location = location;
        Annotation = annotation;
    }

    public int? AnnotatedLine { get; init; }
    public IAnnotation Annotation { get; }
    public IReadOnlyList<string> AnnotationArguments { get; init; }

    public Location Location { get; }

    internal static List<string> AnnotationParametersFromContext(VBAParser.AnnotationContext context, IAnnotation annotation)
    {
        var parameters = new List<string>();
        var argList = context?.annotationArgList();
        if (argList != null)
        {
            parameters.AddRange(annotation.ProcessAnnotationArguments(argList.annotationArg().Select(arg => arg.GetText())));
        }
        return parameters;
    }

    internal static int? ResolveAnnotatedLine(VBAParser.AnnotationContext context)
    {
        var enclosingEndOfStatement = context.GetAncestor<VBAParser.EndOfStatementContext>();

        //Annotations on the same line as non-whitespace statements do not scope to anything.
        if (enclosingEndOfStatement.Start.TokenIndex != 0)
        {
            var firstEndOfLine = enclosingEndOfStatement.GetFirstEndOfLine();
            var parentEndOfLine = context.GetAncestor<VBAParser.EndOfLineContext>();
            if (firstEndOfLine.Equals(parentEndOfLine))
            {
                return null;
            }
        }

        var lastToken = enclosingEndOfStatement.stop;
        return lastToken.Type == VBAParser.NEWLINE
               ? lastToken.Line + 1
               : lastToken.Line;
    }
}
