using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Declarations.Symbols;

public record class RelativeToken
{
    public int DeltaLine { get; init; }
    public int DeltaStartChar { get; init; }
    public int Length { get; init; }
    public int TokenType { get; init; }
    public int TokenModifiers { get; init; }
}

public record class AbsoluteToken
{
    /// <summary>
    /// The 1-based line number where this token begins in the document.
    /// </summary>
    public int StartLine { get; init; }
    /// <summary>
    /// The 1-based line number where this token ends in the document.
    /// </summary>
    /// <remarks>
    /// Some tokens can legally span multiple lines, using line continuations.
    /// </remarks>
    public int EndLine { get; init; }
    /// <summary>
    /// The 0-based column number where this token begins in the document.
    /// </summary>
    public int StartColumn { get; init; }
    /// <summary>
    /// The 0-based column number where this token ends in the document.
    /// </summary>
    public int EndColumn { get; init; }
    /// <summary>
    /// The number of characters taken up by this token.
    /// </summary>
    public int Length { get; init; }
    /// <summary>
    /// Categorizes the token.
    /// </summary>
    public int Type { get; init; }
    /// <summary>
    /// Further categorizes the token.
    /// </summary>
    /// <remarks>
    /// Encoded to a bit flag using a legend based on the index of token modifiers; the encoded value must be less than 65,536.
    /// </remarks>
    public int Modifiers { get; init; }

    public static int[] Encode(AbsoluteToken[] tokens)
    {
        if (tokens.Length == 0)
        {
            return [];
        }

        var encoded = new List<int[]>();
        
        var previous = tokens[0];
        foreach (var token in tokens)
        {
            var relative = (
                deltaLine: token.StartLine - previous.EndLine,
                deltaStartChar: token.StartLine == previous.EndLine ? token.StartColumn : token.StartColumn - previous.EndColumn,
                length: token.Length,
                tokenType: token.Type,
                tokenModifiers: token.Modifiers
            );
            encoded.Add([relative.deltaLine, relative.deltaStartChar, relative.length, relative.tokenType, relative.tokenModifiers]);
        }

        return encoded.SelectMany(e => e).ToArray();
    }

    public static RelativeToken GetToken(int[] encodedTokens, int index) => new()
    {
        DeltaLine = encodedTokens[5 * index],
        DeltaStartChar = encodedTokens[5 * index + 1],
        Length = encodedTokens[5 * index + 2],
        TokenType = encodedTokens[5 * index + 3],
        TokenModifiers = encodedTokens[5 * index + 4]
    };
}
