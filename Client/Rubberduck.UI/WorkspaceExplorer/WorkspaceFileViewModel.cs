using Rubberduck.InternalApi.Model.Workspace;
using System;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceFileViewModel : WorkspaceTreeNodeViewModel
    {
        public static WorkspaceFileViewModel FromModel(File model)
        {
            return new WorkspaceFileViewModel
            {
                Uri = new Uri(model.Uri),
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
    }
}
