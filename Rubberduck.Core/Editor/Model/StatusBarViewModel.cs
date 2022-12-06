using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System;
using System.Windows.Input;

namespace Rubberduck.Core.Editor
{
    public class StatusBarViewModel : ViewModelBase, IStatusBarViewModel
    {
        private int _documentLines;
        public int DocumentLines 
        { 
            get => _documentLines; 
            set
            {
                if (_documentLines != value)
                {
                    _documentLines = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private int _documentLength;
        public int DocumentLength 
        { 
            get => _documentLength;
            set
            {
                if (_documentLength != value) 
                {
                    _documentLength = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretOffset;
        public int CaretOffset
        {
            get => _caretOffset;
            set
            {
                if (_caretOffset != value)
                {
                    _caretOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretLine;
        public int CaretLine
        {
            get => _caretLine;
            set
            {
                if (_caretLine != value)
                {
                    _caretLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _caretColumn;
        public int CaretColumn
        {
            get => _caretColumn;
            set
            {
                if (_caretColumn != value)
                {
                    _caretColumn = value;
                    OnPropertyChanged();
                }
            }
        }

        private QualifiedMemberName _codeLocation;
        public QualifiedMemberName CodeLocation 
        {
            get => _codeLocation;
            set
            {
                if (_codeLocation != value) 
                {
                    _codeLocation = value;   
                    OnPropertyChanged();
                }
            }
        }

        private int _issuesCount;
        public int IssuesCount 
        {
            get => _issuesCount;
            set
            {
                if (_issuesCount != value)
                {
                    _issuesCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _parserState;
        public string ParserState 
        {
            get => _parserState;
            set
            {
                if (_parserState != value)
                {
                    _parserState = value;
                    OnPropertyChanged();
                }
            }
        }
        public int ProgressValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ProgressMaxValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private bool _showDocumentStatusItems;
        public bool ShowDocumentStatusItems 
        {
            get => _showDocumentStatusItems;
            set
            {
                if (_showDocumentStatusItems != value) 
                {
                    _showDocumentStatusItems = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}