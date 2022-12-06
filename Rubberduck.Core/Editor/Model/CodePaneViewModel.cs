using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Listeners;
using Rubberduck.Parsing.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;

namespace Rubberduck.Core.Editor
{
    public class CodePaneViewModel : ViewModelBase, ICodePaneViewModel
    {
        private readonly ICodeParserService _parser;
        private readonly IEditorSettings _settings;

        private IParseTree _parseTree;

        public CodePaneViewModel(ICodeParserService parser, IEditorSettings settings)
        {
            _parser = parser;
            _settings = settings;

            MemberProviders = new ObservableCollection<IMemberProviderViewModel>(
                new[]
                {
                    new MemberProviderViewModel
                    {
                        Name = "(General)",
                        ModuleType = ModuleType.StandardModule,
                        Members = new ObservableCollection<IMemberInfoViewModel>(new MemberInfoViewModel[]
                        {
                            new MemberInfoViewModel()
                            {
                                Name = "(Declarations)",
                                MemberType = MemberType.None,
                            },
                        })
                    }
                });
        }

        public IEditorShellViewModel Shell { get; internal set; }

        private MemberListener _memberListener;
        private VBFoldingListener _foldingListener;
        private TokenStreamRewriter _rewriter;
        public IEnumerable<BlockFoldingInfo> Foldings => _foldingListener?.Foldings ?? Enumerable.Empty<BlockFoldingInfo>();
        public IStatusBarViewModel Status => Shell.Status;

        public async Task ParseAsync(TextReader reader)
        {
            try
            {
                _memberListener = new MemberListener();
                _foldingListener = new VBFoldingListener();
                var listeners = new VBAParserBaseListener[]
                { 
                    _memberListener,
                    _foldingListener,
                };

                Shell.Status.ParserState = "Parsing...";

                var result = await _parser.ParseAsync(Title, reader, listeners);

                Shell.Status.ParserState = "Processing...";

                _parseTree = result.ParseTree;
                _rewriter = result.Rewriter;

                SyntaxErrors = result.Errors.Select(e => new SyntaxErrorViewModel(e));

                OnParseTreeChanged();
                
                Shell.Status.ParserState = "Ready";
            }
            catch
            {
                Shell.Status.ParserState = "Unexpected error";
            }
        }
        private void OnParseTreeChanged()
        {
            ParseTreeChanged?.Invoke(this, new ParseTreeEventArgs(_parseTree, _memberListener.Members, _foldingListener.Foldings));
        }

        private bool _isTabOpen;
        public bool IsTabOpen
        {
            get => _isTabOpen;
            set
            {
                if (_isTabOpen != value) 
                {
                    _isTabOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        public TextDocument Document { get; set; }

        private string _title;
        public string Title 
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _content;
        public string Content 
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }
        public IEditorSettings EditorSettings => _settings;

        private IMemberProviderViewModel _selectedProvider;
        public IMemberProviderViewModel SelectedMemberProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider != value)
                {
                    _selectedProvider?.ClearMemberSelectedHandlers();
                    _selectedProvider = value;
                    OnPropertyChanged();

                    SelectedMemberProviderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedMemberProviderChanged;
        public event EventHandler<ParseTreeEventArgs> ParseTreeChanged;

        public ObservableCollection<IMemberProviderViewModel> MemberProviders { get; set; } 

        public IModuleInfoViewModel ModuleInfo { get; set; }
        public ObservableCollection<IMemberInfoViewModel> Members { get; }

        private IEnumerable<ISyntaxErrorViewModel> _syntaxErrors;
        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors 
        {
            get => _syntaxErrors;
            private set
            {
                if (_syntaxErrors != value) 
                { 
                    _syntaxErrors = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}