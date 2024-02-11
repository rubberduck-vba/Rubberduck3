using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing._v3.Pipeline.Services;

public class SemanticTokensListener : VBAParserBaseListener, IVBListener<AbsoluteToken[]>
{
    private readonly WorkspaceFileUri _workspaceFileUri;

    public SemanticTokensListener(WorkspaceFileUri uri)
    {
        _workspaceFileUri = uri;
    }

    private readonly List<AbsoluteToken> _tokens = [];
    public AbsoluteToken[] Result => _tokens.ToArray();

    private void CreateToken(IToken token, SemanticTokenType tokenType, params SemanticTokenModifier[] modifiers) => CreateToken(token, token, tokenType, modifiers);
    private void CreateToken(IToken startToken, IToken endToken, SemanticTokenType tokenType, params SemanticTokenModifier[] modifiers)
    {
        if (_inModuleHeader)
        {
            modifiers = [.. modifiers, RubberduckSemanticTokenModifier.ModuleHeader];
        }
        _tokens.Add(new AbsoluteToken
        {
            StartLine = startToken.Line,
            EndLine = endToken.EndLine(),
            StartColumn = startToken.Column,
            EndColumn = endToken.EndColumn(),
            Length = endToken.StopIndex - startToken.StartIndex + 1,
            Type = RubberduckSemanticTokenType.TokenTypeId[tokenType],
            Modifiers = modifiers.Select(e => RubberduckSemanticTokenModifier.TokenModifierId[e]).ToHashSet().Sum(),
        });
    }

    private bool _inModuleHeader;

    public override void EnterModuleHeader([NotNull] VBAParser.ModuleHeaderContext context) => _inModuleHeader = true;
    public override void EnterModuleDeclarations([NotNull] VBAParser.ModuleDeclarationsContext context) => _inModuleHeader = false;

    public override void ExitModuleHeader([NotNull] VBAParser.ModuleHeaderContext context)
    {
        var classToken = context.CLASS();
        if (classToken != null)
        {
            CreateToken(context.VERSION().Symbol, RubberduckSemanticTokenType.Keyword);
            CreateToken(classToken.Symbol, RubberduckSemanticTokenType.Keyword);
        }
    }

    public override void ExitTypeHint([NotNull] VBAParser.TypeHintContext context) =>
        CreateToken(context.Start, RubberduckSemanticTokenType.TypeHint, RubberduckSemanticTokenModifier.Deprecated);

    public override void ExitBooleanLiteralIdentifier([NotNull] VBAParser.BooleanLiteralIdentifierContext context) => 
        CreateToken(context.Start, RubberduckSemanticTokenType.BooleanLiteral);

    public override void ExitNumberLiteral([NotNull] VBAParser.NumberLiteralContext context) => 
        CreateToken(context.Start, RubberduckSemanticTokenType.NumberLiteral);

    public override void ExitObjectLiteralIdentifier([NotNull] VBAParser.ObjectLiteralIdentifierContext context) =>
        CreateToken(context.NOTHING().Symbol, RubberduckSemanticTokenType.NothingLiteral);
    
    public override void ExitLiteralExpression([NotNull] VBAParser.LiteralExpressionContext context)
    {
        var dateLiteral = context.DATELITERAL();
        if (dateLiteral != null)
        {
            CreateToken(dateLiteral.Symbol, RubberduckSemanticTokenType.DateLiteral);
        }

        var stringLiteral = context.STRINGLITERAL();
        if (stringLiteral != null)
        {
            CreateToken(stringLiteral.Symbol, RubberduckSemanticTokenType.StringLiteral);
        }
    }

    public override void ExitVariantLiteralIdentifier([NotNull] VBAParser.VariantLiteralIdentifierContext context)
    {
        var emptyLiteral = context.EMPTY();
        if (emptyLiteral != null)
        {
            CreateToken(emptyLiteral.Symbol, RubberduckSemanticTokenType.EmptyLiteral);
        }

        var nullLiteral = context.NULL();
        if (nullLiteral != null)
        {
            CreateToken(nullLiteral.Symbol, RubberduckSemanticTokenType.NullLiteral);
        }
    }

    public override void ExitAttributeStmt([NotNull] VBAParser.AttributeStmtContext context)
    {
        CreateToken(context.ATTRIBUTE().Symbol, RubberduckSemanticTokenType.Keyword, RubberduckSemanticTokenModifier.Attribute);

        IToken attributeNameToken;
        var nameContext = context.attributeName().lExpression();
        if (nameContext is VBAParser.MemberAccessExprContext memberAccessExpr)
        {
            var memberNameToken = memberAccessExpr.lExpression().Start;
            attributeNameToken = memberAccessExpr.unrestrictedIdentifier().start;

            // probably a stretch? adding a 'Declaration' flag to mark it as the token we'd be looking for when e.g. renaming the parent procedure.
            CreateToken(memberNameToken, RubberduckSemanticTokenType.Attribute, RubberduckSemanticTokenModifier.Attribute, RubberduckSemanticTokenModifier.Declaration);
        }
        else
        {
            attributeNameToken = nameContext.Start;
        }
        CreateToken(attributeNameToken, RubberduckSemanticTokenType.Attribute, RubberduckSemanticTokenModifier.Attribute);
    }

    public override void ExitAnnotation([NotNull] VBAParser.AnnotationContext context)
    {
        CreateToken(context.Start, context.Stop, RubberduckSemanticTokenType.Annotation);
    }

    public override void ExitCommentOrAnnotation([NotNull] VBAParser.CommentOrAnnotationContext context)
    {
        var comment = context.comment();
        if (comment != null)
        {
            CreateToken(context.Start, context.Stop, RubberduckSemanticTokenType.Comment);
        }

        var remComment = context.remComment();
        if (remComment != null )
        {
            CreateToken(remComment.REM().Symbol, RubberduckSemanticTokenType.Keyword, RubberduckSemanticTokenModifier.Deprecated);
            
            var body = remComment.commentBody();
            CreateToken(body.Start, body.Stop, RubberduckSemanticTokenType.Comment);
        }

        var annotations = context.annotationList();
        if (annotations != null)
        {
            var body = annotations.commentBody();
            if (body != null)
            {
                CreateToken(body.Start, body.Stop, RubberduckSemanticTokenType.Comment);
            }
        }
    }

    public override void ExitOptionBaseStmt([NotNull] VBAParser.OptionBaseStmtContext context) => 
        CreateToken(context.OPTION_BASE().Symbol, RubberduckSemanticTokenType.Keyword);

    public override void ExitOptionCompareStmt([NotNull] VBAParser.OptionCompareStmtContext context)
    {
        CreateToken(context.OPTION_COMPARE().Symbol, RubberduckSemanticTokenType.Keyword);

        var compareBinary = context.BINARY();
        if (compareBinary != null)
        {
            CreateToken(compareBinary.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var compareText = context.TEXT();
        if (compareText != null) 
        {
            CreateToken(compareText.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var compareDatabase = context.DATABASE();
        if (compareDatabase != null)
        {
            CreateToken(compareDatabase.Symbol, RubberduckSemanticTokenType.Keyword);
        }
    }

    public override void ExitOptionExplicitStmt([NotNull] VBAParser.OptionExplicitStmtContext context) =>
        CreateToken(context.OPTION_EXPLICIT().Symbol, RubberduckSemanticTokenType.Keyword);

    public override void ExitOptionPrivateModuleStmt([NotNull] VBAParser.OptionPrivateModuleStmtContext context) =>
        CreateToken(context.OPTION_PRIVATE_MODULE().Symbol, RubberduckSemanticTokenType.Keyword);

    public override void ExitVariableStmt([NotNull] VBAParser.VariableStmtContext context)
    {
        var dimStmt = context.DIM();
        if (dimStmt != null)
        {
            CreateToken(dimStmt.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var staticStmt = context.STATIC();
        if (staticStmt != null)
        {
            CreateToken(staticStmt.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var visibility = context.visibility();
        if (visibility != null)
        {
            CreateToken(visibility.Start, RubberduckSemanticTokenType.Keyword);
        }
    }

    public override void ExitVariableSubStmt([NotNull] VBAParser.VariableSubStmtContext context)
    {
        var withEvents = context.WITHEVENTS();
        if (withEvents != null)
        {
            CreateToken(withEvents.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        CreateToken(context.identifier().Start, RubberduckSemanticTokenType.Variable, RubberduckSemanticTokenModifier.Declaration);

        var arrayDim = context.arrayDim();
        if (arrayDim != null)
        {
            CreateToken(arrayDim.Start, arrayDim.Stop, RubberduckSemanticTokenType.ArraySubscripts);
        }
    }

    public override void ExitConstStmt([NotNull] VBAParser.ConstStmtContext context) => 
        CreateToken(context.CONST().Symbol, RubberduckSemanticTokenType.Keyword);

    public override void ExitConstSubStmt([NotNull] VBAParser.ConstSubStmtContext context)
    {
        CreateToken(context.identifier().Start, RubberduckSemanticTokenType.Constant, RubberduckSemanticTokenModifier.Declaration);
        CreateToken(context.EQ().Symbol, RubberduckSemanticTokenType.Operator);
    }

    public override void ExitDeclareStmt([NotNull] VBAParser.DeclareStmtContext context)
    {
        CreateToken(context.DECLARE().Symbol, RubberduckSemanticTokenType.Keyword);

        var ptrSafe = context.PTRSAFE();
        if (ptrSafe != null)
        {
            CreateToken(ptrSafe.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        SemanticTokenType tokenType = RubberduckSemanticTokenType.IgnoredExpression;

        var sub = context.SUB();
        if (sub != null)
        {
            tokenType = RubberduckSemanticTokenType.Procedure;
            CreateToken(sub.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var fun = context.FUNCTION();
        if (fun != null)
        {
            tokenType = RubberduckSemanticTokenType.Function;
            CreateToken(fun.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        CreateToken(context.identifier().Start, tokenType, RubberduckSemanticTokenModifier.Declaration);

        var cdecl = context.CDECL();
        if (cdecl != null)
        {
            CreateToken(cdecl.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        CreateToken(context.LIB().Symbol, RubberduckSemanticTokenType.Keyword);

        var alias = context.ALIAS();
        if (alias != null)
        {
            CreateToken(alias.Symbol, RubberduckSemanticTokenType.Keyword);
        }
    }

    public override void ExitArg([NotNull] VBAParser.ArgContext context)
    {
        var optional = context.OPTIONAL();
        if (optional != null)
        {
            CreateToken(optional.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var byref = context.BYREF();
        if (byref != null)
        {
            CreateToken(byref.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var byval = context.BYVAL();
        if (byval != null)
        {
            CreateToken(byval.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        var paramarray = context.PARAMARRAY();
        if (paramarray != null)
        {
            CreateToken(paramarray.Symbol, RubberduckSemanticTokenType.Keyword);
        }

        CreateToken(context.unrestrictedIdentifier().Start, RubberduckSemanticTokenType.Parameter, RubberduckSemanticTokenModifier.Declaration);
    }


    public override void ExitAsTypeClause([NotNull] VBAParser.AsTypeClauseContext context)
    {
        CreateToken(context.AS().Symbol, RubberduckSemanticTokenType.Keyword);

        var asNew = context.NEW();
        if (asNew != null)
        {
            CreateToken(asNew.Symbol, RubberduckSemanticTokenType.Operator);
        }

        var asType = context.type();
        if (asType != null)
        {
            var intrinsicType = asType.baseType();
            if (intrinsicType != null)
            {
                CreateToken(intrinsicType.Start, RubberduckSemanticTokenType.Type);
            }

            var type = asType.complexType();
            if (type != null)
            {
                CreateToken(type.Start, type.Stop, RubberduckSemanticTokenType.Type);
            }

            if (asType.LPAREN() != null)
            {
                CreateToken(asType.LPAREN().Symbol, RubberduckSemanticTokenType.ArraySubscripts);
                CreateToken(asType.RPAREN().Symbol, RubberduckSemanticTokenType.ArraySubscripts);
            }
        }
    }
}
