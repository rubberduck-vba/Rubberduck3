using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Client.LSP
{
    public class LspClientService
    {
        // placeholder code for now...

        public void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var clientProcess = Process.GetCurrentProcess();

            var lsp = LanguageClient.Create(new LanguageClientOptions
            {
                ClientInfo = new ClientInfo
                {
                    //Name = $"[{assembly.Name}]:{clientProcess.Id}",
                    //Version = assembly.Version.ToString(3)
                },
                ClientCapabilities = new ClientCapabilities
                {
                    Workspace = new WorkspaceClientCapabilities
                    {
                        ApplyEdit = true,
                        CodeLens = new Supports<CodeLensWorkspaceClientCapabilities>
                        {
                            Value = new CodeLensWorkspaceClientCapabilities { RefreshSupport = true }
                        },
                        Configuration = new Supports<bool> { Value = true },
                        DidChangeConfiguration = new Supports<DidChangeConfigurationCapability> { Value = new DidChangeConfigurationCapability() },
                        DidChangeWatchedFiles = new Supports<DidChangeWatchedFilesCapability> { Value = new DidChangeWatchedFilesCapability() },
                        ExecuteCommand = new Supports<ExecuteCommandCapability> { Value = new ExecuteCommandCapability() },
                        SemanticTokens = new Supports<SemanticTokensWorkspaceCapability> { Value = new SemanticTokensWorkspaceCapability() },
                        Symbol = new Supports<WorkspaceSymbolCapability> { Value = new WorkspaceSymbolCapability() },
                        WorkspaceEdit = new Supports<WorkspaceEditCapability> { Value = new WorkspaceEditCapability() },
                        WorkspaceFolders = new Supports<bool> { Value = true }
                    },
                    Window = new WindowClientCapabilities
                    {
                        WorkDoneProgress = true,
                        ShowMessage = new Supports<ShowMessageRequestClientCapabilities>
                        {
                            Value = new ShowMessageRequestClientCapabilities
                            {
                                MessageActionItem = new ShowMessageRequestMessageActionItemClientCapabilities
                                {
                                    AdditionalPropertiesSupport = true,
                                }
                            }
                        },
                        ExtensionData = null /*TODO*/,
                    },
                    TextDocument = new TextDocumentClientCapabilities
                    {
                        CallHierarchy = new Supports<CallHierarchyCapability> { Value = new CallHierarchyCapability() },
                        CodeAction = new Supports<CodeActionCapability>
                        {
                            Value = new CodeActionCapability
                            {
                                CodeActionLiteralSupport = new CodeActionLiteralSupportOptions
                                {
                                    CodeActionKind = new CodeActionKindCapabilityOptions { ValueSet = new Container<CodeActionKind>(/*TODO*/) }
                                },
                                DataSupport = true,
                                DisabledSupport = true,
                                IsPreferredSupport = true,
                                ResolveSupport = new CodeActionCapabilityResolveSupportOptions
                                {
                                    Properties = new Container<string>(/*TODO*/)
                                }
                            }
                        },
                        CodeLens = new Supports<CodeLensCapability> { Value = new CodeLensCapability() },
                        ColorProvider = new Supports<ColorProviderCapability> { Value = new ColorProviderCapability() },
                        Completion = new Supports<CompletionCapability> { Value = new CompletionCapability() },
                        Declaration = new Supports<DeclarationCapability> { Value = new DeclarationCapability() },
                        //Definition = null, // we cannot support multiple definitions per declaration
                        DocumentHighlight = new Supports<DocumentHighlightCapability> { Value = new DocumentHighlightCapability() },
                        DocumentLink = new Supports<DocumentLinkCapability> { Value = new DocumentLinkCapability() },
                        DocumentSymbol = new Supports<DocumentSymbolCapability> { Value = new DocumentSymbolCapability() },
                        FoldingRange = new Supports<FoldingRangeCapability> { Value = new FoldingRangeCapability() },
                        Formatting = new Supports<DocumentFormattingCapability> { Value = new DocumentFormattingCapability() },
                        Hover = new Supports<HoverCapability> { Value = new HoverCapability() },
                        Implementation = new Supports<ImplementationCapability> { Value = new ImplementationCapability() },
                        //Moniker = null /*TODO*/, // not sure what these are for TBH
                        OnTypeFormatting = new Supports<DocumentOnTypeFormattingCapability> { Value = new DocumentOnTypeFormattingCapability() },
                        PublishDiagnostics = new Supports<PublishDiagnosticsCapability> { Value = new PublishDiagnosticsCapability() },
                        RangeFormatting = new Supports<DocumentRangeFormattingCapability> { Value = new DocumentRangeFormattingCapability() },
                        References = new Supports<ReferenceCapability> { Value = new ReferenceCapability() },
                        Rename = new Supports<RenameCapability> { Value = new RenameCapability() },
                        SelectionRange = new Supports<SelectionRangeCapability> { Value = new SelectionRangeCapability() },
                        SemanticTokens = new Supports<SemanticTokensCapability> { Value = new SemanticTokensCapability() },
                        SignatureHelp = new Supports<SignatureHelpCapability> { Value = new SignatureHelpCapability() },
                        Synchronization = new Supports<SynchronizationCapability> { Value = new SynchronizationCapability() },
                        TypeDefinition = new Supports<TypeDefinitionCapability> { Value = new TypeDefinitionCapability() },
                        ExtensionData = null /*TODO*/,

                    },
                    Experimental = null /*TODO*/,
                    ExtensionData = null /*TODO*/,
                },

            });

        }

    }
}
