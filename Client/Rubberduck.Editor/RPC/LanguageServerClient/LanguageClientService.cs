using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.ServerPlatform;
using OmniSharp.Extensions.LanguageServer.Client;
using Rubberduck.Editor.RPC.LanguageServerClient.Handlers;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Linq;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Services;

namespace Rubberduck.Editor.RPC.LanguageServerClient
{
    public class LanguageClientService : ServerPlatformClientService
    {
        public LanguageClientService(ILogger<ServerPlatformServiceHelper> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance) 
            : base(logger, settings, workdone, performance)
        {
        }

        protected override void ConfigureClientLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-client.config");
        }

        protected override LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options)
        {
            options
                .WithHandler<PublishDiagnosticsHandler>()

                .WithHandler<LogTraceHandler>()
                .WithHandler<LogMessageHandler>();

            return options;
        }

        protected override ClientCapabilities GetClientCapabilities()
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
                            NormalizesLineEndings = supported, // we must force Windows line endings to sync with VBE
                            ResourceOperations = new(ResourceOperationKind.Create, ResourceOperationKind.Delete, ResourceOperationKind.Rename)
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
                            SymbolKind = new() { ValueSet = new Container<SymbolKind>(Enum.GetValues<RubberduckSymbolKind>().Cast<SymbolKind>()) }
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
                                    nameof(OmniSharp.Extensions.LanguageServer.Protocol.Models.CodeAction.Command),
                                    nameof(OmniSharp.Extensions.LanguageServer.Protocol.Models.CodeAction.Data))
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
                    Definition = new()
                    {
                        // declaration vs definition is unclear (probably useful for overloads?), but "Declaration" will be our primary,
                        // and with "Definitions" we'll accommodate any alternative declarations we come across, perhaps in precompiler-dead code.
                        Value = new() { LinkSupport = supported }
                    }, 
                    DocumentHighlight = new() { Value = new() },
                    DocumentLink = new() { Value = new() { TooltipSupport = supported } },
                    DocumentSymbol = new()
                    {
                        Value = new()
                        {
                            HierarchicalDocumentSymbolSupport = supported,
                            LabelSupport = supported,
                            TagSupport = new() { ValueSet = new Container<SymbolTag>(SymbolTag.Deprecated) },
                            SymbolKind = new()
                            {
                                ValueSet = new Container<SymbolKind>(Enum.GetValues<RubberduckSymbolKind>().Cast<SymbolKind>())
                            }
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
                    PublishDiagnostics = new() { Value = new() { VersionSupport = true, CodeDescriptionSupport = true, DataSupport = true, RelatedInformation = true, TagSupport = new(true) } },
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
                            AugmentsSyntaxTokens = false, // client does not know about syntax tokens, initial highlighting is approximated via regex rules
                            ServerCancelSupport = supported,
                            OverlappingTokenSupport = false,
                            Requests = new()
                            {
                                // configured like this, we're going to be requesting tokens for full documents, not dealing with deltas.
                                Full = new() { Value = new() { Delta = false } },
                                //Range = new() { Value = new() } // could enable requesting tokens for a particular range in a document. maybe later.
                            },
                            // these two are fully extensible:
                            TokenTypes = new Container<SemanticTokenType>(RubberduckSemanticTokenType.SemanticTokenTypes.OrderBy(e => e.Id).Select(e => e.TokenType)),
                            TokenModifiers = new Container<SemanticTokenModifier>(RubberduckSemanticTokenModifier.SemanticTokenModifiers.OrderBy(e => e.Id).Select(e => e.TokenModifier)),
                        }
                    },
                    SelectionRange = new()
                    {
                        Value = new()
                        {
                            LineFoldingOnly = supported, // we don't fold partial lines.
                            RangeLimit = 10000, // larger than that can't be sync'd with VBE.
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
