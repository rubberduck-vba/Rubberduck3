using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.WorkspaceExplorer;
using System;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{
    public class WorkspaceFileViewModel : WorkspaceTreeNodeViewModel, IWorkspaceFileViewModel
    {
        public static WorkspaceFileViewModel FromModel(File model, Uri srcRoot)
        {
            return new WorkspaceFileViewModel
            {
                Uri = new Uri(srcRoot, model.Uri),
                Name = model.Name,
                IsAutoOpen = model.IsAutoOpen,
                IsInProject = true
            };
        }

        private bool _isAutoOpen;
        public bool IsAutoOpen
        {
            get => _isAutoOpen;
            set
            {
                if (_isAutoOpen != value)
                {
                    _isAutoOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
