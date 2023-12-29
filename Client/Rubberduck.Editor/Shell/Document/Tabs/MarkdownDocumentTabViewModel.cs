﻿using Rubberduck.SettingsProvider.Model.Editor;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using System;

namespace Rubberduck.Editor.Shell.Document.Tabs
{
    /// <summary>
    /// A view model for a type of document tab that contains a markdown document.
    /// </summary>
    public class MarkdownDocumentTabViewModel : DocumentTabViewModel
    {
        public MarkdownDocumentTabViewModel(Uri documentUri, string title, string content, bool isReadOnly, 
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand)
            : base(documentUri, "md/html", title, content, isReadOnly, showSettingsCommand, closeToolWindowCommand)
        {
            SettingKey = nameof(EditorSettings);
        }

        public override SupportedDocumentType DocumentType => SupportedDocumentType.MarkdownDocument;

        private bool _showPreview = true;
        public bool ShowPreview
        {
            get => _showPreview;
            set
            {
                if (_showPreview != value)
                {
                    _showPreview = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasLinksEnabled = false;
        public bool EnableHyperlinks
        {
            get => _hasLinksEnabled;
            set
            {
                if (_hasLinksEnabled != value)
                {
                    _hasLinksEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
