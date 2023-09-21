using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using System.Threading.Tasks;
using System.IO.Pipes;
using Nerdbank.Streams;
using Rubberduck.InternalApi.ServerPlatform;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Client
{
    public class LanguageClientService
    {
        public static Process StartServerProcess<TServer>(TServer server, TransportType transport, InitializeTrace traceLevel, int clientProcessId = default, string pipeName = default)
            where TServer : IServerProcess
        {
            var args = string.Empty;
            switch (transport)
            {
                case TransportType.StdIO:
                    args = $"StdIO";
                    break;
                case TransportType.Pipe:
                    args = $"Pipe --client {clientProcessId}";
                    if (!string.IsNullOrWhiteSpace(pipeName))
                    {
                        args += $" --name \"{pipeName}\"";
                    }
                    break;
                default:
                    break;
            }

            if (traceLevel == InitializeTrace.Verbose)
            {
                args += " --verbose";
            }
            else if(traceLevel == InitializeTrace.Off)
            {
                args += " --silent";
            }

            return server.Start(transport, hidden: true, args);
        }

        public static LanguageClientOptions ConfigureLanguageClient(Assembly clientAssembly, NamedPipeClientStream pipe, InitializeTrace traceLevel)
        {
            var options = ConfigureLanguageClient(clientAssembly, traceLevel);
            options.WithInput(pipe.UsePipeReader());
            options.WithOutput(pipe.UsePipeWriter());

            return options;
        }

        public static LanguageClientOptions ConfigureLanguageClient(Assembly clientAssembly, Process serverProcess, InitializeTrace traceLevel)
        {
            var options = ConfigureLanguageClient(clientAssembly, traceLevel);
            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            return options;
        }

        private static LanguageClientOptions ConfigureLanguageClient(Assembly clientAssembly, InitializeTrace traceLevel)
        {
            var options = new LanguageClientOptions();

            var info = clientAssembly.ToClientInfo();
            var workspace = new DirectoryInfo(clientAssembly.Location).ToWorkspaceFolder();
            var clientCapabilities = GetClientCapabilities();

            options
                .WithClientInfo(info)
                .WithClientCapabilities(clientCapabilities)
                .WithWorkspaceFolder(workspace)
                .WithContentModifiedSupport(true)
                .WithTrace(traceLevel)
                .OnInitialize(async (client, param, token) =>
                {
                    var logger = client.GetRequiredService<ILogger<LanguageClientService>>();
                    await Task.CompletedTask;
                })
                .OnInitialized(async (client, param, response, token) =>
                {
                    var logger = client.GetRequiredService<ILogger<LanguageClientService>>();
                    logger.LogInformation("Sending Initialized notification...");

                    client.SendLanguageProtocolInitialized(new InitializedParams());
                    await Task.CompletedTask;
                })
                .OnStarted(async (client, token) =>
                {
                    var logger = client.GetRequiredService<ILogger<LanguageClientService>>();
                    logger.LogInformation("Ready.");
                    await Task.CompletedTask;
                })
                .OnLogMessage((param, token) =>
                {
                    //var logger = client.GetRequiredService<ILogger<LanguageClientService>>(); /* well isn't this interesting! */
                    //logger.LogInformation("Sending Initialized notification...");
                    Debug.WriteLine($"[{param.Type}] {param.Message}");
                })
                .OnWorkDoneProgressCreate(async (param, token) =>
                {
                    // TODO store WorkDoneProgress token
                    await Task.CompletedTask;
                })
                .OnProgress(async (param, token) =>
                {
                    // TODO actually handle these
                    Debug.WriteLine($"{param.Value}");
                    await Task.CompletedTask;
                })
                .OnShowMessage(async (param, token) =>
                {
                    // TODO show in a message box
                    Debug.WriteLine($"[{param.Type}] {param.Message}");
                    await Task.CompletedTask;
                })
                .OnShowMessageRequest(async (param, token) =>
                {
                    // TODO setup and display message box prompt
                    // TODO return message action item
                    return await Task.FromResult(param.Actions.FirstOrDefault());
                })
                .OnLogTrace((param, token) =>
                {
                    // TODO surface messages to "console" message window
                    switch (traceLevel)
                    {
                        case InitializeTrace.Off:
                            break;
                        case InitializeTrace.Messages:
                            Debug.WriteLine($"[TRACE] {param.Message}");
                            break;
                        case InitializeTrace.Verbose:
                            Debug.WriteLine($"[TRACE] {param.Message} | {param.Verbose}");
                            break;
                    }
                })
                .OnApplyWorkspaceEdit(async (param, token) =>
                {
                    // TODO actually apply worksapce edit
                    var response = new ApplyWorkspaceEditResponse
                    {
                        Applied = false,
                    };
                    return await Task.FromResult(response);
                })
                .OnTelemetryEvent(async (param, token) =>
                {
                    // TODO relay data to telemetry server
                    await Task.CompletedTask;
                })
                .OnWorkspaceFolders(async (param, token) =>
                {
                    // TODO return actual client workspace folders
                    return await Task.FromResult(new Container<WorkspaceFolder>(workspace));
                })
                .OnPublishDiagnostics(async (param, token) =>
                {
                    // TODO refresh diagnostics for the specified document URI
                    await Task.CompletedTask;
                })
                .OnSemanticTokensRefresh(async (param, token) =>
                {
                    // TODO ??
                    await Task.CompletedTask;
                })
            ;

            return options;
        }

        private static ClientCapabilities GetClientCapabilities()
        {
            var supported = new Supports<bool> { Value = true };
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

                    CodeLens = new Supports<CodeLensWorkspaceClientCapabilities>
                    {
                        Value = new CodeLensWorkspaceClientCapabilities { RefreshSupport = supported }
                    },
                    DidChangeConfiguration = new Supports<DidChangeConfigurationCapability>
                    {
                        Value = new DidChangeConfigurationCapability()
                    },
                    DidChangeWatchedFiles = new Supports<DidChangeWatchedFilesCapability>
                    {
                        Value = new DidChangeWatchedFilesCapability()
                    },
                    ExecuteCommand = new Supports<ExecuteCommandCapability>
                    {
                        Value = new ExecuteCommandCapability()
                    },
                    FileOperations = new Supports<FileOperationsWorkspaceClientCapabilities>
                    {
                        Value = new FileOperationsWorkspaceClientCapabilities
                        {
                            DidCreate = supported,
                            DidDelete = supported,
                            DidRename = supported,
                        }
                    },
                    WorkspaceEdit = new Supports<WorkspaceEditCapability>
                    {
                        Value = new WorkspaceEditCapability
                        {
                            ChangeAnnotationSupport = new WorkspaceEditSupportCapabilitiesChangeAnnotationSupport { GroupsOnLabel = supported },
                            DocumentChanges = supported,
                            FailureHandling = FailureHandlingKind.TextOnlyTransactional,
                            //NormalizesLineEndings = supported,
                            //ResourceOperations = supported
                        }
                    },
                    SemanticTokens = new Supports<SemanticTokensWorkspaceCapability>
                    {
                        Value = new SemanticTokensWorkspaceCapability { RefreshSupport = true }
                    },
                    Symbol = new Supports<WorkspaceSymbolCapability>
                    {
                        Value = new WorkspaceSymbolCapability
                        {
                            TagSupport = new Supports<TagSupportCapabilityOptions> { Value = new TagSupportCapabilityOptions { ValueSet = new Container<SymbolTag>(SymbolTag.Deprecated) } },
                            SymbolKind = new SymbolKindCapabilityOptions { ValueSet = new Container<SymbolKind>(SymbolKind.Array, SymbolKind.Boolean, SymbolKind.Class, SymbolKind.Constant, SymbolKind.Enum, SymbolKind.EnumMember, SymbolKind.Event, SymbolKind.Field, SymbolKind.File, SymbolKind.Function, SymbolKind.Interface, SymbolKind.Method, SymbolKind.Module, SymbolKind.Number, SymbolKind.Null, SymbolKind.Object, SymbolKind.Operator, SymbolKind.Package, SymbolKind.Property, SymbolKind.String, SymbolKind.Struct, SymbolKind.Variable) }
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* WINDOW */

                Window = new WindowClientCapabilities
                {
                    WorkDoneProgress = supported,
                    ShowDocument = new Supports<ShowDocumentClientCapabilities> { Value = new ShowDocumentClientCapabilities { Support = supported } },
                    ShowMessage = new Supports<ShowMessageRequestClientCapabilities>
                    {
                        Value = new ShowMessageRequestClientCapabilities
                        {
                            MessageActionItem = new ShowMessageRequestMessageActionItemClientCapabilities { AdditionalPropertiesSupport = supported }
                        }
                    },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* DOCUMENT */

                TextDocument = new TextDocumentClientCapabilities
                {
                    CallHierarchy = new Supports<CallHierarchyCapability> { Value = new CallHierarchyCapability() },
                    CodeAction = new Supports<CodeActionCapability>
                    {
                        Value = new CodeActionCapability
                        {
                            CodeActionLiteralSupport = new CodeActionLiteralSupportOptions
                            {
                                CodeActionKind = new CodeActionKindCapabilityOptions
                                {
                                    ValueSet = new Container<CodeActionKind>()
                                }
                            },
                            DataSupport = supported,
                            DisabledSupport = supported,
                            IsPreferredSupport = supported,
                            HonorsChangeAnnotations = supported,
                            ResolveSupport = new CodeActionCapabilityResolveSupportOptions
                            {
                                Properties = new Container<string>(nameof(CodeAction.Command), nameof(CodeAction.Data))
                            }
                        }
                    },
                    CodeLens = null, // for now anyway
                    ColorProvider = new Supports<ColorProviderCapability> { Value = new ColorProviderCapability() },
                    Completion = new Supports<CompletionCapability>
                    {
                        Value = new CompletionCapability
                        {
                            CompletionItem = new CompletionItemCapabilityOptions
                            {
                                CommitCharactersSupport = supported,
                                DeprecatedSupport = supported,
                                InsertReplaceSupport = supported,
                                PreselectSupport = supported,
                                ResolveAdditionalTextEditsSupport = supported,
                                SnippetSupport = supported,
                                TagSupport = new Supports<CompletionItemTagSupportCapabilityOptions>
                                {
                                    Value = new CompletionItemTagSupportCapabilityOptions
                                    {
                                        ValueSet = new Container<CompletionItemTag>(CompletionItemTag.Deprecated)
                                    }
                                },
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown),
                                InsertTextModeSupport = new CompletionItemInsertTextModeSupportCapabilityOptions
                                {
                                    ValueSet = new Container<InsertTextMode>(InsertTextMode.AdjustIndentation)
                                },
                                ResolveSupport = new CompletionItemCapabilityResolveSupportOptions
                                {
                                    Properties = new Container<string>(nameof(CompletionItem.Data), nameof(CompletionItem.Detail), nameof(CompletionItem.Documentation), nameof(CompletionItem.Command)),
                                }
                            },
                            ContextSupport = supported,
                            CompletionItemKind = new CompletionItemKindCapabilityOptions
                            {
                                ValueSet = new Container<CompletionItemKind>(CompletionItemKind.Class, CompletionItemKind.Color, CompletionItemKind.Constant, CompletionItemKind.Enum, CompletionItemKind.EnumMember, CompletionItemKind.Event, CompletionItemKind.Field, CompletionItemKind.File, CompletionItemKind.Folder, CompletionItemKind.Function, CompletionItemKind.Interface, CompletionItemKind.Keyword, CompletionItemKind.Method, CompletionItemKind.Module, CompletionItemKind.Operator, CompletionItemKind.Property, CompletionItemKind.Reference, CompletionItemKind.Snippet, CompletionItemKind.Struct, CompletionItemKind.Text, CompletionItemKind.Value, CompletionItemKind.Variable)
                            }
                        }
                    },
                    Declaration = new Supports<DeclarationCapability>
                    {
                        Value = new DeclarationCapability { LinkSupport = supported }
                    },
                    Definition = null,
                    DocumentHighlight = new Supports<DocumentHighlightCapability> { Value = new DocumentHighlightCapability() },
                    DocumentLink = new Supports<DocumentLinkCapability> { Value = new DocumentLinkCapability { TooltipSupport = supported } },
                    DocumentSymbol = new Supports<DocumentSymbolCapability>
                    {
                        Value = new DocumentSymbolCapability
                        {
                            HierarchicalDocumentSymbolSupport = supported,
                            LabelSupport = supported,
                            TagSupport = new TagSupportCapabilityOptions { ValueSet = new Container<SymbolTag>(SymbolTag.Deprecated) },
                            SymbolKind = new SymbolKindCapabilityOptions { ValueSet = new Container<SymbolKind>(SymbolKind.Array, SymbolKind.Boolean, SymbolKind.Class, SymbolKind.Constant, SymbolKind.Enum, SymbolKind.EnumMember, SymbolKind.Event, SymbolKind.Field, SymbolKind.File, SymbolKind.Function, SymbolKind.Interface, SymbolKind.Method, SymbolKind.Module, SymbolKind.Number, SymbolKind.Null, SymbolKind.Object, SymbolKind.Operator, SymbolKind.Package, SymbolKind.Property, SymbolKind.String, SymbolKind.Struct, SymbolKind.Variable) }
                        }
                    },
                    FoldingRange = new Supports<FoldingRangeCapability>
                    {
                        Value = new FoldingRangeCapability
                        {
                            LineFoldingOnly = false,
                            RangeLimit = 10000 // no module can exceed 10K LOC
                        }
                    },
                    Formatting = new Supports<DocumentFormattingCapability> { Value = new DocumentFormattingCapability() },
                    Hover = new Supports<HoverCapability> { Value = new HoverCapability { ContentFormat = new Container<MarkupKind>(MarkupKind.Markdown) } },
                    Implementation = new Supports<ImplementationCapability> { Value = new ImplementationCapability { LinkSupport = supported } },
                    LinkedEditingRange = null, // for now
                    OnTypeFormatting = new Supports<DocumentOnTypeFormattingCapability> { Value = new DocumentOnTypeFormattingCapability() },
                    PublishDiagnostics = new Supports<PublishDiagnosticsCapability> { Value = new PublishDiagnosticsCapability() },
                    References = new Supports<ReferenceCapability> { Value = new ReferenceCapability() },
                    Rename = new Supports<RenameCapability>
                    {
                        Value = new RenameCapability
                        {
                            PrepareSupport = supported,
                            //HonorsChangeAnnotations = supported,
                            PrepareSupportDefaultBehavior = PrepareSupportDefaultBehavior.Identifier,
                        }
                    },
                    RangeFormatting = new Supports<DocumentRangeFormattingCapability> { Value = new DocumentRangeFormattingCapability() },
                    SignatureHelp = new Supports<SignatureHelpCapability>
                    {
                        Value = new SignatureHelpCapability
                        {
                            ContextSupport = supported,
                            SignatureInformation = new SignatureInformationCapabilityOptions
                            {
                                ActiveParameterSupport = supported,
                                DocumentationFormat = new Container<MarkupKind>(MarkupKind.Markdown),
                                ParameterInformation = new SignatureParameterInformationCapabilityOptions
                                {
                                    LabelOffsetSupport = supported,
                                }
                            }
                        }
                    },
                    SemanticTokens = new Supports<SemanticTokensCapability>
                    {
                        Value = new SemanticTokensCapability
                        {
                            Formats = new Container<SemanticTokenFormat>(SemanticTokenFormat.Defaults),
                            MultilineTokenSupport = supported,
                            OverlappingTokenSupport = false,
                            Requests = new SemanticTokensCapabilityRequests
                            {
                                Full = new Supports<SemanticTokensCapabilityRequestFull> { Value = new SemanticTokensCapabilityRequestFull { Delta = false } },
                                Range = new Supports<SemanticTokensCapabilityRequestRange> { Value = new SemanticTokensCapabilityRequestRange() }
                            },
                            //TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier...) /* TODO FIGURE THIS ONE OUT */
                            //TokenTypes = new Container<SemanticTokenType>(SemanticTokenType...)
                        }
                    },
                    TypeDefinition = new Supports<TypeDefinitionCapability> { Value = new TypeDefinitionCapability { LinkSupport = supported } },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* OTHER */

                Experimental = new Dictionary<string, JToken>(),
                ExtensionData = new Dictionary<string, JToken>()
            };

            return capabilities;
        }
    }
}
