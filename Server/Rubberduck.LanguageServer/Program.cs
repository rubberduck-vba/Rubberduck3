using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Rubberduck.LanguageServer.Configuration;
using Rubberduck.ServerPlatform;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Rubberduck.ServerPlatform.Model.LocalDb;

namespace Rubberduck.LanguageServer
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            /*TODO accept command-line arguments*/
            await StartAsync();

            return 0;
        }

        private static async Task StartAsync()
        {
            Console.WriteLine(ServerSplash.GetRenderString(Assembly.GetExecutingAssembly().GetName()));

            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource))
            ;

            var host = builder.Build();
            await host.RunAsync(tokenSource.Token);
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource tokenSource)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs
            var config = new ServerCapabilities
            {
                Workspace = new WorkspaceServerCapabilities
                {
                    WorkspaceFolders = new DidChangeWorkspaceFolderRegistrationOptions.StaticOptions
                    {
                        Supported = true,
                        ChangeNotifications = true,
                    },
                    FileOperations = new FileOperationsWorkspaceServerCapabilities
                    {
                        DidCreate = new DidCreateFileRegistrationOptions.StaticOptions
                        {
                            // TODO define filters
                        },
                        DidDelete = new DidDeleteFileRegistrationOptions.StaticOptions
                        // TODO define filters
                        {
                        },
                        DidRename = new DidRenameFileRegistrationOptions.StaticOptions
                        {
                            // TODO define filters
                        },
                        WillCreate = new WillCreateFileRegistrationOptions.StaticOptions
                        {
                            // TODO define filters
                        },
                        WillDelete = new WillDeleteFileRegistrationOptions.StaticOptions
                        {
                            // TODO define filters
                        },
                        WillRename = new WillRenameFileRegistrationOptions.StaticOptions
                        {
                            // TODO define filters
                        },
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },
                WorkspaceSymbolProvider = new WorkspaceSymbolRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                TextDocumentSync = new TextDocumentSync(TextDocumentSyncKind.Full)
                {
                    Options = new TextDocumentSyncOptions
                    {
                        Change = TextDocumentSyncKind.Full,
                        OpenClose = true,
                        Save = true,
                        WillSave = true,
                        WillSaveWaitUntil = true
                    },
                },
                SemanticTokensProvider = new SemanticTokensRegistrationOptions.StaticOptions
                {
                    Id = "RD3.LanguageServer.SemanticTokens",
                    WorkDoneProgress = true,
                    Full = new SemanticTokensCapabilityRequestFull { Delta = false },
                    Range = new SemanticTokensCapabilityRequestRange { },
                    Legend = new SemanticTokensLegend
                    {
                        TokenModifiers = new[]
                        {
                            SemanticTokenModifier.Declaration,
                            SemanticTokenModifier.DefaultLibrary,
                            SemanticTokenModifier.Documentation,
                            SemanticTokenModifier.Deprecated,
                            SemanticTokenModifier.Abstract,
                            SemanticTokenModifier.Static,
                        },
                        TokenTypes = new[]
                        {
                            SemanticTokenType.Class,
                            SemanticTokenType.Comment,
                            SemanticTokenType.Enum,
                            SemanticTokenType.EnumMember,
                            SemanticTokenType.Event,
                            SemanticTokenType.Function,
                            SemanticTokenType.Interface,
                            SemanticTokenType.Keyword,
                            SemanticTokenType.Label,
                            SemanticTokenType.Method,
                            SemanticTokenType.Number,
                            SemanticTokenType.Modifier,
                            SemanticTokenType.Operator,
                            SemanticTokenType.Parameter,
                            SemanticTokenType.Property,
                            SemanticTokenType.String,
                            SemanticTokenType.Struct,
                            SemanticTokenType.Type,
                            SemanticTokenType.Variable,
                        }
                    }
                },
                DocumentSymbolProvider = new DocumentSymbolRegistrationOptions.StaticOptions
                {
                    WorkDoneProgress = true,
                    Label = "TODO",
                },
                DocumentHighlightProvider = new DocumentHighlightRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                DocumentFormattingProvider = new DocumentFormattingRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                DocumentLinkProvider = new DocumentLinkRegistrationOptions.StaticOptions { WorkDoneProgress = true, ResolveProvider = true },
                DocumentOnTypeFormattingProvider = new DocumentOnTypeFormattingRegistrationOptions.StaticOptions { WorkDoneProgress = true, FirstTriggerCharacter = "\n" },
                DocumentRangeFormattingProvider = new DocumentRangeFormattingRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                CallHierarchyProvider = new CallHierarchyRegistrationOptions.StaticOptions { WorkDoneProgress = true, Id = "RD3.LanguageServer.CallHierarchy" }, // TODO
                CodeActionProvider = new CodeActionRegistrationOptions.StaticOptions 
                { 
                    WorkDoneProgress= true,
                    ResolveProvider = true,
                    CodeActionKinds = new[]
                    {
                        CodeActionKind.Source,
                        CodeActionKind.QuickFix,
                        CodeActionKind.Refactor,
                        CodeActionKind.RefactorExtract,
                        CodeActionKind.RefactorInline,
                        CodeActionKind.RefactorRewrite,
                    }
                },
                CodeLensProvider = new CodeLensRegistrationOptions.StaticOptions { ResolveProvider = true, WorkDoneProgress = true },
                ColorProvider = new DocumentColorRegistrationOptions.StaticOptions { WorkDoneProgress = true, Id = "RD3.LanguageServer.Color" },
                CompletionProvider = new CompletionRegistrationOptions.StaticOptions
                {
                    WorkDoneProgress = true,
                    ResolveProvider= true,
                    //TriggerCharacters = new[] {"."},
                    //AllCommitCharacters = new[] { "\t", "\n", " " },
                },
                DeclarationProvider = new DeclarationRegistrationOptions.StaticOptions { WorkDoneProgress = true, Id = "RD3.LanguageServer.Declaration" },
                ExecuteCommandProvider = new ExecuteCommandRegistrationOptions.StaticOptions 
                { 
                    WorkDoneProgress = true,
                    //Commands = new[] { "" }, // TODO
                },
                Experimental = new Dictionary<string, JToken>(),
                ExtensionData = new Dictionary<string, JToken>(),
                FoldingRangeProvider = new FoldingRangeRegistrationOptions.StaticOptions { WorkDoneProgress =true, Id = "RD3.LanguageServer.FoldingRange" },
                HoverProvider = new HoverRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                ImplementationProvider = new ImplementationRegistrationOptions.StaticOptions { WorkDoneProgress = true, Id = "RD3.LanguageServer.Implementation" },
                ReferencesProvider = new ReferenceRegistrationOptions.StaticOptions { WorkDoneProgress = true },
                RenameProvider = new RenameRegistrationOptions.StaticOptions { PrepareProvider = true, WorkDoneProgress = true, Id = "RD3.LanguageServer.Rename" },
                SelectionRangeProvider = new SelectionRangeRegistrationOptions.StaticOptions { WorkDoneProgress = true, Id = "RD3.LanguageServer.SelectionRange" },
                SignatureHelpProvider = new SignatureHelpRegistrationOptions.StaticOptions 
                {
                    WorkDoneProgress = true,
                    TriggerCharacters = new string[] { "(", "," },
                    //RetriggerCharacters = new string[] { "(", "," }, // TODO
                },
               /* TODO */
            };

            services.AddRubberduckServerServices(config, tokenSource);
        }
    }
}