﻿using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.Editor;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;

namespace Rubberduck.Editor.Shell.Document
{
    /// <summary>
    /// A view model for a type of document tab that contains a markdown document.
    /// </summary>
    public class MarkdownDocumentTabViewModel : DocumentTabViewModel
    {
        public MarkdownDocumentTabViewModel(DocumentState state, bool isReadOnly,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            IDocumentStatusViewModel activeDocumentStatus)
            : base(state, isReadOnly, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus)
        {
            SettingKey = nameof(ShowWelcomeTabSetting);
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
