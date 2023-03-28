//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.Client;
using Rubberduck.Common;
using Rubberduck.Common.Hotkeys;
using Rubberduck.Core;
using Rubberduck.Core.About;
using Rubberduck.Core.Editor;
using Rubberduck.Core.Editor.Tools;
using Rubberduck.Core.WebApi;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.InternalApi.UIContext;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.TokenStreamProviders;
using Rubberduck.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.ComManagement.NonDisposingDecorators;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using Rubberduck.VBEditor.ComManagement.TypeLibs.Abstract;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.SourceCodeHandling;
using Rubberduck.VBEditor.UI;
using Rubberduck.VBEditor.UI.OfficeMenus;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using Rubberduck.VBEditor.VbeRuntime;
using Rubberduck.VersionCheck;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

namespace Rubberduck.Root
{
    internal class MenuBuilder
    {
        private readonly Type _itemType;
        private readonly List<Type> _childItemTypes = new List<Type>();
        private readonly IServiceCollection _services;

        public MenuBuilder(IServiceCollection services)
            : this(services, typeof(RubberduckParentMenu)) { }

        public MenuBuilder(IServiceCollection services,  Type itemType)
        {
            _services = services;
            _itemType = itemType;
        }

        public MenuBuilder WithCommandMenuItem<TMenuItem, TCommandInterface, TCommandImpl>() 
            where TMenuItem : class, ICommandMenuItem 
            where TCommandInterface : class, IMenuCommand
            where TCommandImpl : class, TCommandInterface
        {
            _services.AddTransient<TMenuItem>();
            _services.AddSingleton<TCommandInterface, TCommandImpl>();

            return this;
        }
    }

    internal class RubberduckServicesBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public IServiceProvider Build() => _services.BuildServiceProvider();
        public RubberduckServicesBuilder WithAddIn(IVBE vbe, IAddIn addin)
        {
            _services.AddSingleton<IVBE>(vbe);
            _services.AddSingleton<IAddIn>(addin);
            _services.AddSingleton<ICommandBars>(new CommandBarsNonDisposingDecorator<ICommandBars>(vbe.CommandBars));
            
            var repository = new ProjectsRepository(vbe);
            _services.AddSingleton<IProjectsProvider>(repository);
            //_services.AddSingleton<IProjectsRepository>(repository);

            return this;
        }

        public RubberduckServicesBuilder WithApplication()
        {
            _services.AddSingleton<App>();
            _services.AddSingleton<IConfigurationService<Configuration>, ConfigurationLoader>();
            return this;
        }

        public RubberduckServicesBuilder WithLanguageServer()
        {
            var supported = new Supports<bool> { Value = true };

            var client = Assembly.GetExecutingAssembly().ToClientInfo();
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
                            WillCreate = supported,
                            WillDelete = supported,
                            WillRename = supported
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
                                Full = new Supports<SemanticTokensCapabilityRequestFull> { Value = new SemanticTokensCapabilityRequestFull{ Delta = false } },
                                Range = new Supports<SemanticTokensCapabilityRequestRange> { Value = new SemanticTokensCapabilityRequestRange() }
                            },
                            //TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier...) /* TODO FIGURE THIS ONE OUT */
                            //TokenTypes = new Container<SemanticTokenType>(SemanticTokenType...)
                        }
                    },
                    Synchronization = new Supports<SynchronizationCapability>
                    {
                        Value = new SynchronizationCapability
                        {
                            DidSave = supported,
                            WillSave = supported,
                            WillSaveWaitUntil = supported,
                        }
                    },
                    TypeDefinition = new Supports<TypeDefinitionCapability> { Value = new TypeDefinitionCapability { LinkSupport = supported } },
                    ExtensionData = new Dictionary<string, JToken>()
                },

                /* OTHER */

                Experimental = new Dictionary<string, JToken>(),
                ExtensionData = new Dictionary<string, JToken>()
            };

            var workspace = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).ToWorkspaceFolder();

            _services.ConfigureLanguageClient(client, capabilities, workspace);

            return this;
        }

        public RubberduckServicesBuilder WithMsoCommandBarMenu()
        {
            _services.AddTransient<IAppMenu, RubberduckParentMenu>();

            return this;
        }

        public RubberduckServicesBuilder WithRubberduckEditor()
        {
            _services.AddSingleton<IDockablePresenter, EditorShellDockablePresenter>();
            _services.AddSingleton<IEditorShellWindowProvider, EditorShellWindowProvider>();

            _services.AddSingleton<IEditorShellViewModel, EditorShellViewModel>();
            _services.AddSingleton<IShellToolTabProvider, ShellToolTabProvider>();
            _services.AddSingleton<IStatusBarViewModel, StatusBarViewModel>();

            _services.AddSingleton<ISyncPanelToolTab, SyncPanelToolTab>();
            _services.AddSingleton<ISyncPanelViewModel, SyncPanelViewModel>();
            _services.AddSingleton<ISyncPanelModuleViewModelProvider, SyncPanelModuleViewModelProvider>();

            return this;
        }

        public RubberduckServicesBuilder WithAssemblyInfo()
        {
            _services.AddSingleton<Version>(_ => Assembly.GetExecutingAssembly().GetName().Version);
            _services.AddSingleton<IOperatingSystem, WindowsOperatingSystem>();

            return this;
        }

        public RubberduckServicesBuilder WithParser()
        {
            _services.AddSingleton<ICommonTokenStreamProvider<TextReader>, TextReaderTokenStreamProvider>();

            return this;
        }

        public RubberduckServicesBuilder WithFileSystem(IVBE vbe)
        {
            _services.AddSingleton<IFileSystem, FileSystem>();
            _services.AddSingleton<ITempSourceFileHandler>(vbe.TempSourceFileHandler);
            _services.AddSingleton<IPersistencePathProvider>(PersistencePathProvider.Instance);

            return this;
        }

        public RubberduckServicesBuilder WithSettingsProvider()
        {
            _services.AddSingleton<GeneralConfigProvider>();
            _services.AddSingleton<IConfigurationService<GeneralSettings>, GeneralConfigProvider>();
            _services.AddSingleton<IPersistenceService<GeneralSettings>, XmlPersistenceService<GeneralSettings>>();

            // TODO refactor settings / simplify abstractions

            return this;
        }

        public RubberduckServicesBuilder WithNativeServices(IVBE vbe)
        {
            var nativeApi = new VbeNativeApiAccessor();
            _services.AddSingleton<IVbeNativeApi>(nativeApi);
            _services.AddSingleton<IBeepInterceptor>(new BeepInterceptor(nativeApi));
            _services.AddSingleton<IVbeEvents>(VbeEvents.Initialize(vbe));
            _services.AddSingleton<IVBETypeLibsAPI, VBETypeLibsAPI>();

            _services.AddSingleton<IUiDispatcher, UiDispatcher>();
            _services.AddSingleton<IUiContextProvider>(UiContextProvider.Instance());

            _services.AddSingleton<IRubberduckHooks, RubberduckHooks>();
            _services.AddSingleton<HotkeyFactory>();

            return this;
        }

        public RubberduckServicesBuilder WithVersionCheck()
        {
            _services.AddSingleton<VersionCheckCommand>();
            _services.AddSingleton<IPublicApiClient, PublicApiClient>();
            _services.AddSingleton<IVersionCheckService>(provider => new VersionCheckService(provider.GetRequiredService<IPublicApiClient>(), Assembly.GetExecutingAssembly().GetName().Version));

            return this;
        }

        public RubberduckServicesBuilder WithCommands()
        {
            _services.AddSingleton<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddSingleton<ShowEditorShellCommandMenuItem>();

            _services.AddSingleton<IAboutCommand, AboutCommand>();
            _services.AddSingleton<AboutCommandMenuItem>();
            _services.AddSingleton<IWebNavigator, WebNavigator>();
            _services.AddSingleton<IMessageBox, FormsMessageBox>(); // TODO implement a WpfMessageBox

            return this;
        }
    }
}
