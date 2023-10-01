using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Nerdbank.Streams;
using Rubberduck.Client.Extensions;
using Rubberduck.Client.Handlers;
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Client
{
    public class LanguageClientService
    {
        public static LanguageClientOptions ConfigureLanguageClient(Assembly clientAssembly, NamedPipeClientStream pipe, long clientProcessId, InitializeTrace traceLevel)
        {
            var options = new LanguageClientOptions();
            options.WithInput(pipe.UsePipeReader());
            options.WithOutput(pipe.UsePipeWriter());

            ConfigureLanguageClient(options, clientAssembly, clientProcessId, traceLevel);
            return options;
        }

        public static LanguageClientOptions ConfigureLanguageClient(Assembly clientAssembly, Process serverProcess, long clientProcessId, InitializeTrace traceLevel)
        {
            var options = new LanguageClientOptions();
            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            options.DisableDynamicRegistration();
            options.EnableProgressTokens();
            options.EnableWorkspaceFolders();

            options.WithSerializer(new OmniSharp.Extensions.LanguageServer.Protocol.Serialization.LspSerializer(ClientVersion.Lsp3));
            options.ConfigureLogging(ConfigureClientLogging);

            ConfigureLanguageClient(options, clientAssembly, clientProcessId, traceLevel);
            return options;
        }

        private static void ConfigureClientLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-client.config");
        }

        private static LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options, Assembly clientAssembly, long clientProcessId, InitializeTrace traceLevel)
        {
            var info = clientAssembly.ToClientInfo();
            var workspace = new DirectoryInfo(clientAssembly.Location).ToWorkspaceFolder();
            var clientCapabilities = GetClientCapabilities();

            var initializationOptions = new InitializationOptions
            {
                Timestamp = DateTime.Now,
                ClientProcessId = clientProcessId,
                // TODO
            };

            options
                .WithClientInfo(info)
                .WithClientCapabilities(clientCapabilities)
                .WithTrace(traceLevel)
                .WithInitializationOptions(initializationOptions)
                .WithWorkspaceFolder(workspace)
                .WithContentModifiedSupport(true)

                .OnInitialize((ILanguageClient client, InitializeParams request, CancellationToken cancellationToken) =>
                {

                    return Task.CompletedTask;
                })

                .WithHandler<WorkspaceFoldersHandler>()
                .WithHandler<WorkDoneProgressCreateHandler>()
                .WithHandler<WorkDoneProgressCancelHandler>()
                .WithHandler<WorkDoneProgressHandler>()

                .WithHandler<LogTraceHandler>()
                .WithHandler<LogMessageHandler>()
                .WithHandler<ShowMessageHandler>()
                .WithHandler<ShowMessageRequestHandler>()

                .WithHandler<ApplyWorkspaceEditHandler>()
                .WithHandler<PublishDiagnosticsHandler>()
                .WithHandler<SemanticTokensRefreshHandler>()

                .WithHandler<TelemetryEventHandler>()
            ;

            return options;
        }

        private static ClientCapabilities GetClientCapabilities()
        {
            var supported = new Supports<bool> { Value = true };
            //var notSupported = new Supports<bool> { Value = false };

            var capabilities = new ClientCapabilities
            {
                /* GENERAL */

                General = new GeneralClientCapabilities
                {
                    Markdown = new MarkdownClientCapabilities { /* parser, version */ },
                    RegularExpressions = new RegularExpressionsClientCapabilities { Engine = "VBScript", Version = "5.5" },
                },

                /* WORKSPACE */

                Workspace = new WorkspaceClientCapabilities
                {
                    Configuration = supported,
                    ApplyEdit = supported,
                    WorkspaceFolders = supported,

                    CodeLens = new()
                    {
                        Value = new() { RefreshSupport = supported }
                    },
                    DidChangeConfiguration = new()
                    {
                        Value = new()
                    },
                    DidChangeWatchedFiles = new()
                    {
                        Value = new()
                    },
                    ExecuteCommand = new()
                    {
                        Value = new()
                    },
                    FileOperations = new()
                    {
                        Value = new()
                        {
                            DidCreate = supported,
                            DidDelete = supported,
                            DidRename = supported,
                        }
                    },
                    WorkspaceEdit = new()
                    {
                        Value = new()
                        {
                            ChangeAnnotationSupport = new() { GroupsOnLabel = supported },
                            DocumentChanges = supported,
                            FailureHandling = FailureHandlingKind.TextOnlyTransactional,
                            //NormalizesLineEndings = supported,
                            //ResourceOperations = supported
                        }
                    },
                    SemanticTokens = new()
                    {
                        Value = new() { RefreshSupport = true }
                    },
                    Symbol = new()
                    {
                        Value = new()
                        {
                            TagSupport = new() { Value = new() { ValueSet = new Container<SymbolTag>(SymbolTag.Deprecated) } },
                            SymbolKind = new() { ValueSet = new Container<SymbolKind>(SymbolKind.Array, SymbolKind.Boolean, SymbolKind.Class, SymbolKind.Constant, SymbolKind.Enum, SymbolKind.EnumMember, SymbolKind.Event, SymbolKind.Field, SymbolKind.File, SymbolKind.Function, SymbolKind.Interface, SymbolKind.Method, SymbolKind.Module, SymbolKind.Number, SymbolKind.Null, SymbolKind.Object, SymbolKind.Operator, SymbolKind.Package, SymbolKind.Property, SymbolKind.String, SymbolKind.Struct, SymbolKind.Variable) }
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* WINDOW */

                Window = new WindowClientCapabilities
                {
                    WorkDoneProgress = supported,
                    ShowDocument = new() { Value = new() { Support = supported } },
                    ShowMessage = new()
                    {
                        Value = new()
                        {
                            MessageActionItem = new() { AdditionalPropertiesSupport = supported }
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* DOCUMENT */

                TextDocument = new()
                {
                    CallHierarchy = new() { Value = new() },
                    CodeAction = new()
                    {
                        Value = new()
                        {
                            CodeActionLiteralSupport = new()
                            {
                                CodeActionKind = new()
                                {
                                    ValueSet = new Container<CodeActionKind>(/*TODO*/)
                                }
                            },
                            DataSupport = supported,
                            DisabledSupport = supported,
                            IsPreferredSupport = supported,
                            HonorsChangeAnnotations = supported,
                            ResolveSupport = new()
                            {
                                Properties = new Container<string>(
                                    nameof(CodeAction.Command), 
                                    nameof(CodeAction.Data))
                            }
                        }
                    },
                    CodeLens = null, // planned post-3.0
                    ColorProvider = new() { Value = new() },
                    Completion = new()
                    {
                        Value = new()
                        {
                            CompletionItem = new()
                            {
                                CommitCharactersSupport = supported,
                                DeprecatedSupport = supported,
                                InsertReplaceSupport = supported,
                                PreselectSupport = supported,
                                ResolveAdditionalTextEditsSupport = supported,
                                SnippetSupport = supported,
                                TagSupport = new()
                                {
                                    Value = new()
                                    {
                                        ValueSet = new Container<CompletionItemTag>(CompletionItemTag.Deprecated)
                                    }
                                },
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown),
                                InsertTextModeSupport = new()
                                {
                                    ValueSet = new Container<InsertTextMode>(InsertTextMode.AdjustIndentation)
                                },
                                ResolveSupport = new()
                                {
                                    Properties = new Container<string>(
                                        nameof(CompletionItem.Data), 
                                        nameof(CompletionItem.Detail), 
                                        nameof(CompletionItem.Documentation), 
                                        nameof(CompletionItem.Command)),
                                }
                            },
                            ContextSupport = supported,
                            CompletionItemKind = new()
                            {
                                ValueSet = new Container<CompletionItemKind>(
                                    CompletionItemKind.Class, 
                                    CompletionItemKind.Color, 
                                    CompletionItemKind.Constant, 
                                    CompletionItemKind.Enum, 
                                    CompletionItemKind.EnumMember, 
                                    CompletionItemKind.Event, 
                                    CompletionItemKind.Field, 
                                    CompletionItemKind.File, 
                                    CompletionItemKind.Folder, 
                                    CompletionItemKind.Function, 
                                    CompletionItemKind.Interface, 
                                    CompletionItemKind.Keyword, 
                                    CompletionItemKind.Method, 
                                    CompletionItemKind.Module, 
                                    CompletionItemKind.Operator, 
                                    CompletionItemKind.Property, 
                                    CompletionItemKind.Reference, 
                                    CompletionItemKind.Snippet, 
                                    CompletionItemKind.Struct, 
                                    CompletionItemKind.Text, 
                                    CompletionItemKind.Value, 
                                    CompletionItemKind.Variable)
                            }
                        }
                    },
                    Declaration = new()
                    {
                        Value = new() { LinkSupport = supported }
                    },
                    Definition = null, // declaration vs definition is unclear
                    DocumentHighlight = new() { Value = new() },
                    DocumentLink = new() { Value = new() { TooltipSupport = supported } },
                    DocumentSymbol = new()
                    {
                        Value = new()
                        {
                            HierarchicalDocumentSymbolSupport = supported,
                            LabelSupport = supported,
                            TagSupport = new() { ValueSet = new Container<SymbolTag>(SymbolTag.Deprecated) },
                            SymbolKind = new() { ValueSet = new Container<SymbolKind>(
                                SymbolKind.Array, 
                                SymbolKind.Boolean, 
                                SymbolKind.Class, 
                                SymbolKind.Constant, 
                                SymbolKind.Enum, 
                                SymbolKind.EnumMember, 
                                SymbolKind.Event, 
                                SymbolKind.Field, 
                                SymbolKind.File, 
                                SymbolKind.Function, 
                                SymbolKind.Interface, 
                                SymbolKind.Method, 
                                SymbolKind.Module, 
                                SymbolKind.Number, 
                                SymbolKind.Null, 
                                SymbolKind.Object, 
                                SymbolKind.Operator, 
                                SymbolKind.Package, 
                                SymbolKind.Property, 
                                SymbolKind.String, 
                                SymbolKind.Struct, 
                                SymbolKind.Variable) }
                        }
                    },
                    Diagnostic = new() { Value = new() { RelatedDocumentSupport = supported } },
                    FoldingRange = new()
                    {
                        Value = new()
                        {
                            LineFoldingOnly = false,
                            RangeLimit = 10000 // VBA will not compile a module that exceeds 10K LOC
                        }
                    },
                    Formatting = new() { Value = new() },
                    Hover = new() { Value = new() { ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown) } },
                    Implementation = new() { Value = new() { LinkSupport = supported } },
                    OnTypeFormatting = new() { Value = new() },
                    PublishDiagnostics = new() { Value = new() },
                    References = new() { Value = new() },
                    Rename = new()
                    {
                        Value = new()
                        {
                            PrepareSupport = supported,
                            //HonorsChangeAnnotations = supported,
                            PrepareSupportDefaultBehavior = PrepareSupportDefaultBehavior.Identifier
                        }
                    },
                    RangeFormatting = new() { Value = new() },
                    SignatureHelp = new()
                    {
                        Value = new()
                        {
                            ContextSupport = supported,
                            SignatureInformation = new()
                            {
                                ActiveParameterSupport = supported,
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown),
                                ParameterInformation = new() { LabelOffsetSupport = supported }
                            }
                        }
                    },
                    SemanticTokens = new()
                    {
                        Value = new()
                        {
                            Formats = new Container<SemanticTokenFormat>(SemanticTokenFormat.Defaults),
                            MultilineTokenSupport = supported,
                            OverlappingTokenSupport = false,
                            Requests = new()
                            {
                                Full = new() { Value = new() { Delta = false } },
                                //Range = new() { Value = new() }
                            },
                            //TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier...) /* TODO FIGURE THIS ONE OUT */
                            //TokenTypes = new Container<SemanticTokenType>(SemanticTokenType...)
                        }
                    },
                    SelectionRange = new()
                    {
                        Value = new()
                        {
                            LineFoldingOnly = supported,
                            RangeLimit = 10000,
                        },
                    },
                    Synchronization = new()
                    {
                        Value = new()
                        {
                            DidSave = true,
                            WillSave = false,
                            WillSaveWaitUntil = false,
                        }
                    },
                    TypeDefinition = new() { Value = new() { LinkSupport = supported } },
                    TypeHierarchy = new() { Value = new TypeHierarchyCapability() },

                    ExtensionData = new Dictionary<string, JToken>(),
                },

                /* OTHER */

                Experimental = new Dictionary<string, JToken>(),
                ExtensionData = new Dictionary<string, JToken>()
            };

            return capabilities;
        }
    }
}
