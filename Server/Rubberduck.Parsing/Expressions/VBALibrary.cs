﻿using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing.Expressions;

public static class VBALibrary
{
    private static readonly Dictionary<string, Func<IExpression, IExpression>> _libraryFunctions = new()
    {
        { "INT", expr => new IntLibraryFunctionExpression(expr) },
        { "FIX", expr => new FixLibraryFunctionExpression(expr) },
        { "ABS", expr => new AbsLibraryFunctionExpression(expr) },
        { "SGN", expr => new SgnLibraryFunctionExpression(expr) },
        { "LEN", expr => new LenLibraryFunctionExpression(expr) },
        { "LENB", expr => new LenBLibraryFunctionExpression(expr) },
        { "CBOOL", expr => new CBoolLibraryFunctionExpression(expr) },
        { "CBYTE", expr => new CByteLibraryFunctionExpression(expr) },
        { "CCUR", expr => new CCurLibraryFunctionExpression(expr) },
        { "CDBL", expr => new CDblLibraryFunctionExpression(expr) },
        { "CINT", expr => new CIntLibraryFunctionExpression(expr) },
        { "CLNG", expr => new CLngLibraryFunctionExpression(expr) },
        { "CLNGLNG", expr => new CLngLngLibraryFunctionExpression(expr) },
        { "CLNGPTR", expr => new CLngPtrLibraryFunctionExpression(expr) },
        { "CSNG", expr => new CSngLibraryFunctionExpression(expr) },
        { "CDATE", expr => new CDateLibraryFunctionExpression(expr) },
        { "CSTR", expr => new CStrLibraryFunctionExpression(expr) },
        { "CVAR", expr => new CVarLibraryFunctionExpression(expr) }
    };

    public static IExpression CreateLibraryFunction(string functionName, IExpression argument)
    {
        if (_libraryFunctions.TryGetValue(functionName.ToUpper(), out var functionCreator))
        {
            return functionCreator.Invoke(argument);
        }
        throw new InvalidOperationException("Unexpected library function encountered: " + functionName);
    }
}
