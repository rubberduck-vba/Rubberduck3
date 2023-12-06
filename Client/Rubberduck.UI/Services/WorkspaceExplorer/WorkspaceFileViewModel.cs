using Rubberduck.InternalApi.Model.Workspace;
using System;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceFileViewModel : WorkspaceTreeNodeViewModel
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

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    OnPropertyChanged();
                }
            }
        }


    }
}
