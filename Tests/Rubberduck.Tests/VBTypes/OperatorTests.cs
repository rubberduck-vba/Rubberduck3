﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Model.Declarations.Execution;
using Rubberduck.InternalApi.Model.Declarations.Operators.Abstract;
using Rubberduck.InternalApi.Model.Declarations.Symbols;
using Rubberduck.InternalApi.Model.Declarations.Types;
using Rubberduck.InternalApi.Model.Declarations.Types.Abstract;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.IO.Abstractions;
using Visibility = Rubberduck.InternalApi.Model.Accessibility;

namespace Rubberduck.Tests.VBTypes;

public abstract class OperatorTests : ServiceBaseTest
{
    protected static readonly WorkspaceUri ParentProjectUri = new WorkspaceFolderUri(null, new("test://project"));
    protected static readonly WorkspaceUri ParentModuleUri = new WorkspaceFileUri("module", ParentProjectUri);
    protected static readonly WorkspaceUri ParentProcedureUri = new WorkspaceFileUri("module/procedure", ParentProjectUri);

    protected static readonly ProcedureSymbol ParentProcedureSymbol = new("Procedure", ParentProcedureUri, Visibility.Public);
    protected static VBProcedureMember ParentProcedure = new(ParentProcedureUri, "Procedure", RubberduckSymbolKind.Procedure, Visibility.Public, ParentProcedureSymbol, isUserDefined: true);
    protected static readonly VBStdModuleType ParentModule = new("Module", ParentModuleUri, members: [ParentProcedure]);

    protected override void ConfigureServices(IServiceCollection services)
    {
        ((Mock<IFileSystem>)Mocks[typeof(IFileSystem)]).Setup(m => m.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "some path");

        base.ConfigureServices(services);
        services.AddScoped<VBExecutionContext>();
    }

    protected VBBinaryOperator CreateOperator(ref VBProcedureMember scope, TypedSymbol lhs, TypedSymbol rhs)
    {
        var procedureSymbol = ParentProcedureSymbol.WithChildren([lhs, rhs]);
        var parentProcedure = ParentProcedure.WithDeclaration(procedureSymbol);
        return CreateOperator(parentProcedure.Uri, lhs, rhs);
    }
    protected abstract VBBinaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol lhs, TypedSymbol rhs);

    protected VBUnaryOperator CreateOperator(ref VBProcedureMember scope, TypedSymbol symbol)
    {
        var procedureSymbol = ParentProcedureSymbol.WithChildren([symbol]);
        var parentProcedure = ParentProcedure.WithDeclaration(procedureSymbol);
        return CreateOperator(parentProcedure.Uri, symbol);
    }
    protected abstract VBUnaryOperator CreateOperator(WorkspaceUri uri, TypedSymbol symbol);

    protected void OutputExecutionScope(VBExecutionScope scope, bool outputNames = true, bool verboseDiagnostics = false)
    {
        Console.WriteLine($"Scope: [{scope.MemberInfo.Kind}] {scope.MemberInfo.Uri}");
        Console.WriteLine($"x64: {scope.Is64BitHost}");
        Console.WriteLine($"Option Compare {scope.OptionCompare}");
        Console.WriteLine($"Option Explicit {(scope.OptionExplicit ? "On" : "Off")}");
        Console.WriteLine($"Option Strict {(scope.OptionStrict ? "On" : "Off")}");
        if (outputNames)
        {
            Console.WriteLine($"Symbol names: [{string.Join(',', scope.Names)}]");
        }

        Console.WriteLine($"Diagnostics:");
        foreach (var diagnostic in scope.Diagnostics)
        {
            Console.WriteLine($"**[{diagnostic.Code!.Value.String}] {diagnostic.Message}");
            if (verboseDiagnostics)
            {
                Console.WriteLine(diagnostic.ToString());
            }
        }

        Console.WriteLine($"Active OnErrorRedirect: {(scope.ActiveOnErrorResumeNext ? "ResumeNext" : (scope.ActiveOnErrorGoTo != null ? $"GoTo {scope.ActiveOnErrorGoTo.Name}" : "(none)"))}");
        Console.WriteLine($"Active ErrorState: {scope.ActiveErrorState}");
        if (scope.ActiveErrorState || scope.Error != null)
        {
            Console.WriteLine($"Error details:");
            Console.WriteLine($"{scope.Error!}");
        }
    }

    protected TypedSymbol CreateVariable(VBExecutionContext context, VBType type, string name,
        Visibility accessibility = Visibility.Implicit,
        string? asTypeToken = null)
    {
        var asType = asTypeToken is null ? null : $"{Tokens.As} {asTypeToken}";
        var symbol = new VariableDeclarationSymbol(name, ParentProcedureUri, accessibility, asType).WithResolvedType(type);

        if (type is VBVariantType variant)
        {
            type = variant.Subtype;
        }
        context.SetSymbolValue(symbol, type.DefaultValue);
        return symbol;
    }

    protected TypedSymbol CreateClassType(VBExecutionContext context, string name, bool isUserDefined = true, bool isCreatable = true)
    {
        var type = new VBClassType(name, new Uri(ParentProjectUri.OriginalString + $"/{name}"), isUserDefined) { IsCreatable = isCreatable };
        return new ClassTypeInstanceSymbol(Visibility.Undefined, name, ParentProjectUri).WithResolvedType(type);
    }
}
