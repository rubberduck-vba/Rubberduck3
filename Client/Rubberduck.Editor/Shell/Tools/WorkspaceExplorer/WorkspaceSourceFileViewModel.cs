using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.WorkspaceExplorer;
using System;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{

    public class WorkspaceSourceFileViewModel : WorkspaceFileViewModel, IWorkspaceSourceFileViewModel
    {
        public static WorkspaceSourceFileViewModel FromModel(Module model, Uri srcRoot)
        {

            return new WorkspaceSourceFileViewModel
            {
                Uri = new Uri(srcRoot, model.Uri),
                Name = model.Name,
                IsAutoOpen = model.IsAutoOpen,
                DocumentClassType = model.Super,
                IsInProject = true
            };
        }

        private DocClassType? _documentClassType;
        public DocClassType? DocumentClassType
        {
            get => _documentClassType;
            set
            {
                if (_documentClassType != value)
                {
                    _documentClassType = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
