using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Message;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System.Threading.Tasks;

namespace Rubberduck.Main.Commands.ShowRubberduckEditor
{
    class ShowRubberduckEditorCommand : ComCommandBase, IShowRubberduckEditorCommand
    {
        private readonly IVBE _vbe;
        private readonly IProjectsRepository _vbeProjects;

        private readonly IMessageService _message;
        private readonly EditorClientApp _client;

        public ShowRubberduckEditorCommand(UIServiceHelper service, IVbeEvents vbeEvents, 
            EditorClientApp clientApp,
            IMessageService message,
            IVBE vbe,
            IProjectsRepository repository)
            : base(service, vbeEvents)
        {
            _message = message;
            _client = clientApp;

            _vbe = vbe;
            _vbeProjects = repository;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            Service.LogTrace("Starting Rubberduck Editor language client...");
            await _client.StartupAsync(Service.Settings.LanguageClientSettings.StartupSettings);
        }

        private bool CanStartEditorForActiveProject(out string? workspace)
        {
            workspace = null;

            var projectId = _vbe.ActiveVBProject?.ProjectId;
            _vbeProjects.Refresh();

            var settings = Service.Settings.LanguageClientSettings;

            foreach (var (id, project) in _vbeProjects.Projects())
            {
                if (projectId is null || id == projectId)
                {
                    var hostDocumentFullFileName = project.FileName; // e.g. "C:\\Dev\\Battleship.xlsm"
                    var defaultWorkspaceRoot = settings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath;

                    if (!string.IsNullOrWhiteSpace(hostDocumentFullFileName))
                    {
                        // project is saved.

                        if (settings.WorkspaceSettings.RequireDefaultWorkspaceRootHost)
                        {
                            if (!hostDocumentFullFileName.StartsWith(defaultWorkspaceRoot))
                            {
                                // host document should be saved under the default workspace root folder;
                                if (ConfirmCreateDefaultWorkspaceRoot(defaultWorkspaceRoot, hostDocumentFullFileName))
                                {
                                    var hostFile = System.IO.Path.GetFileNameWithoutExtension(hostDocumentFullFileName);
                                    workspace = System.IO.Path.Combine(defaultWorkspaceRoot, hostFile);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            // workspace root could be anywhere; let it be wherever the host document is currently saved.
                            var directory = new System.IO.DirectoryInfo(hostDocumentFullFileName);
                            if (!hostDocumentFullFileName.StartsWith(defaultWorkspaceRoot)) 
                            {
                                // confirm workspace root path if it doesn't exist and isn't under the default workspace root folder:
                                if (!directory.Exists && ConfirmCreateWorkspaceFolder(directory.FullName))
                                {
                                    workspace = directory.FullName;
                                    return true;
                                }
                            }
                            else
                            {
                                workspace = directory.FullName;
                                return true;
                            }
                        }
                    }
                    else if (settings.RequireSavedHost)
                    {
                        // hosted VBA project is not saved, but settings require a saved host.
                        MessageSavedHostIsRequired();
                    }
                    else if (ConfirmStartWithoutWorkspace())
                    {
                        // hosted VBA project is not saved: start without a workspace root (editor will prompt to create one).
                        return true;
                    }

                    break;
                }
            }

            return false;
        }

        private bool ConfirmCreateDefaultWorkspaceRoot(string defaultWorkspaceRoot, string hostDocumentFullFileName)
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = true, // default workspace root is safe to automatically use
                ResourceKey = "AcceptAction_ConfirmCreateDefaultWorkspaceRoot",
                ToolTipKey = "ToolTip_ConfirmCreateDefaultWorkspaceRoot",
            };

            var hostFile = System.IO.Path.GetFileName(hostDocumentFullFileName);
            var workspaceName = System.IO.Path.GetFileNameWithoutExtension(hostFile);

            var model = new MessageRequestModel
            {
                Key = "Workspace_DefaultWorkspaceRootRequired",
                Title = "Default Workspace Root Required",
                Message = "The current configuration requires that the host document for the workspace/project be saved under the *default workspace root* folder. **Create a new workspace folder for this project?**",
                Verbose = $"The host document will be saved under `{defaultWorkspaceRoot}\\{workspaceName}\\{hostFile}`.", // oh yeah, how?
                MessageActions = [acceptAction, MessageAction.CancelAction],
                Level = LogLevel.Warning
            };

            return _message.ShowMessageRequest(model)?.MessageAction == acceptAction;
        }

        private bool ConfirmCreateWorkspaceFolder(string path)
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = false, // we cannot assume we can/should automatically be creating workspace folders
                ResourceKey = "AcceptAction_ConfirmCreateWorkspace",
                ToolTipKey = "ToolTip_ConfirmCreateWorkspace",
            };

            var model = new MessageRequestModel
            {
                Key = "Workspace_ConfirmCreateWorkspace",
                Title = "Create Workspace",
                Message = $"This will create a new Rubberduck workspace under folder `{path}`.",
                Verbose = $"The folder will contain a `{ProjectFile.FileName}` Rubberduck project file and a `{ProjectFile.SourceRoot}` folder where the source files will be exported. Consider using the same local root folder for all Rubberduck projects/workspaces.",
                Level = LogLevel.Information,
                MessageActions = [acceptAction, MessageAction.DefaultCancelAction],
            };

            return _message.ShowMessageRequest(model)?.MessageAction == acceptAction;
        }

        private void MessageSavedHostIsRequired()
        {
            var model = new MessageModel
            {
                Key = "Workspace_SavedHostDocumentRequired",
                Title = "Unsaved Host Document",
                Message = "The current configuration requires that the host document for the workspace/project be saved to disk first.",
                Level = LogLevel.Warning
            };

            _message.ShowMessage(model, provider => provider.OkOnly());
        }

        private bool ConfirmStartWithoutWorkspace()
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = true, // inconsequential, but still a more advanced use case; host document will not sync.
                ResourceKey = "AcceptAction_ConfirmStartWithoutWorkspace",
                ToolTipKey = "ToolTip_ConfirmStartWithoutWorkspace",
            };

            var model = new MessageRequestModel
            {
                Key = "Workspace_NoWorkspaceForUnsavedHost",
                Title = "No Workspace?",
                Message = "The host document is not saved. Start the editor without a workspace?",
                MessageActions = [acceptAction, MessageAction.CancelAction],
                Level = LogLevel.Information
            };

            return _message.ShowMessageRequest(model, provider =>
                [
                    provider.FromMessageAction(acceptAction),
                    provider.FromMessageAction(MessageAction.CancelAction)
                ])?.MessageAction == acceptAction;
        }
    }
}