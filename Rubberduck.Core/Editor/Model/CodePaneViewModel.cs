using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Listeners;
using Rubberduck.Parsing.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rubberduck.Core.Editor
{
    public class CodePaneViewModel : ViewModelBase, ICodePaneViewModel
    {
        private readonly IModuleParser _parser;
        private readonly IEditorSettings _settings;

        private IParseTree _parseTree;

        public CodePaneViewModel(/*ICodeParserService parser,*/ IEditorSettings settings, IEnumerable<IMemberProviderViewModel> memberProviders)
        {
//            _parser = parser;
            _settings = settings;

            MemberProviders = new ObservableCollection<IMemberProviderViewModel>(memberProviders);
            CloseCommand = new DelegateCommand(null, p => EditorShellContext.Current.Shell.UnloadModule((QualifiedModuleName)p)); // TODO handle sync to VBE when dirty
        }

        private VBFoldingListener _foldingListener;
        private TokenStreamRewriter _rewriter;

        public ICommand CloseCommand { get; }

        public IEnumerable<BlockFoldingInfo> Foldings => _foldingListener?.Foldings ?? Enumerable.Empty<BlockFoldingInfo>();
        public IStatusBarViewModel Status => EditorShellContext.Current.Shell.Status;

        public async Task ParseAsync(TextReader reader)
        {
            try
            {
                _foldingListener = new VBFoldingListener(_settings.BlockFoldingSettings);
                var listeners = new VBAParserBaseListener[]
                { 
                    _foldingListener,
                };

                EditorShellContext.Current.Shell.Status.ParserState = "Parsing...";
                
                var sw = Stopwatch.StartNew();
//                var result = await _parser.ParseAsync(ModuleInfo.Name, reader, listeners);
                sw.Stop();

//                _parseTree = result.ParseTree;
//                _rewriter = result.Rewriter;

//                SyntaxErrors = result.Errors.Select(e => new SyntaxErrorViewModel(e));

                EditorShellContext.Current.Shell.Status.ParserState = $"Parse completed: {sw.ElapsedMilliseconds:N0}ms";

                var args = new ParseTreeEventArgs(_parseTree, null, _foldingListener.Foldings, SyntaxErrors);
                OnParseTreeChanged(args);
                if (!args.SyntaxErrors.Any()) 
                {
                    OnParseTreeInspectionsRequested(args);
                }
            }
            catch
            {
                EditorShellContext.Current.Shell.Status.ParserState = "Unexpected error";
            }
        }

        private void OnParseTreeChanged(ParseTreeEventArgs e)
        {
            ParseTreeChanged?.Invoke(this, e);
        }

        private void OnParseTreeInspectionsRequested(ParseTreeEventArgs e)
        {
            ParseTreeInspectionsRequested?.Invoke(this, e);
        }

        private IModuleInfoViewModel _moduleInfo;
        public IModuleInfoViewModel ModuleInfo
        {
            get => _moduleInfo;
            set
            {
                if (_moduleInfo != value)
                {
                    _moduleInfo = value;
                    OnPropertyChanged();
                }
            }
        }


        public TextDocument Document { get; set; }

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
        public event EventHandler<ParseTreeEventArgs> ParseTreeInspectionsRequested;

        public ObservableCollection<IMemberProviderViewModel> MemberProviders { get; set; } 

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