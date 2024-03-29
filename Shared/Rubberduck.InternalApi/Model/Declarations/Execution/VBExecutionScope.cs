﻿using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations.Execution.Values;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rubberduck.InternalApi.Model.Declarations.Execution;

public enum VBOptionCompare
{
    Binary,
    Text,
    Database
}

public record class VBExecutionScope : IDiagnosticSource, IExecutable
{
    private readonly Stack<VBExecutionScope> _callStack;
    private readonly Dictionary<Symbol, VBTypedValue> _symbols;
    
    public VBExecutionScope(Stack<VBExecutionScope> callStack, Dictionary<Symbol, VBTypedValue> symbolTable, VBTypeMember member, VBRuntimeErrorException? error = null, Diagnostic[]? diagnostics = null)
    {
        _callStack = callStack;
        _symbols = symbolTable;

        MemberInfo = member;
        Error = error;

        Diagnostics = diagnostics ?? [];
        Names = symbolTable.Select(e => e.Key.Name).ToImmutableHashSet();
    }

    public ImmutableHashSet<string> Names { get; }

    public bool Is64BitHost { get; init; }
    public bool OptionStrict { get; init; }
    public bool OptionExplicit { get; init; }
    public VBOptionCompare OptionCompare { get; init; }

    public VBTypedValue GetTypedValue(Symbol symbol) => _symbols[symbol];
    public void SetTypedValue(Symbol symbol, VBTypedValue value) => _symbols[symbol] = value;

    public VBTypeMember MemberInfo { get; init; }

    public VBRuntimeErrorException? Error { get; init; }
    public bool ActiveOnErrorResumeNext { get; init; }
    public Symbol? ActiveOnErrorGoTo { get; init; }
    public Symbol? ActiveGoSubReturnTo { get; init; }
    public bool ActiveErrorState => Error != null;

    public IEnumerable<Diagnostic> Diagnostics { get; init; }

    public VBExecutionScope WithError(VBRuntimeErrorException error) => WithDiagnostics(error.Diagnostics) with { Error = error };
    public VBExecutionScope WithDiagnostics(IEnumerable<Diagnostic> diagnostics) => this with { Diagnostics = Diagnostics.Concat(diagnostics).ToArray() };
    public VBExecutionScope WithDiagnostic(Diagnostic diagnostic) => this with { Diagnostics = Diagnostics.Append(diagnostic).ToArray() };

    public VBTypedValue? Execute(ref VBExecutionContext context, bool rethrow = false)
    {
        try
        {
            var scope = context.CurrentScope;
            // TODO map IExecutable symbols to an instructions table, implement traversal, jumps, loops, conditionals

            context.ExitScope();
            return Evaluate(ref scope);
        }
        catch (VBCompileErrorException vbCompileError)
        {
            context.AddDiagnostic(RubberduckDiagnostic.CompileError(vbCompileError));
            if (rethrow)
            {
                throw;
            }
        }
        catch (VBRuntimeErrorException vbRuntimeError)
        {
            var scope = context.CurrentScope;
            if (scope.ActiveOnErrorGoTo != null)
            {
                scope = scope with { Error = vbRuntimeError };
                // TODO implement an InstructionPointer and give it the ActiveOnErrorGoTo executable symbol.
            }
            else if (!scope.ActiveOnErrorResumeNext)
            {
                context.ExitScope(vbRuntimeError);
            }

            if (rethrow)
            {
                throw;
            }
        }
        return null;
    }

    public VBTypedValue? Evaluate(ref VBExecutionScope context, bool rethrow = false) => context.GetTypedValue(MemberInfo.Declaration!);
}
