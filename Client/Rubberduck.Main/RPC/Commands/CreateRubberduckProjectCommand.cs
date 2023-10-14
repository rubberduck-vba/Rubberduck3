using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged;
using System;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.Main.RPC.Commands
{



    /// <summary>
    /// A command that is sent from the editor to the add-in, to
    /// export the active project from the VBE into workspace folders and generate the .rdproj file.
    /// </summary>
    /// <remarks>
    /// If the host supports more than a single project, each project must have its own workspace folder.
    /// </remarks>
    public sealed class CreateRubberduckProjectCommand : RpcCommandBase
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;
        private readonly IProjectsProvider _projectsProvider;

        public CreateRubberduckProjectCommand(ILogger<CreateRubberduckProjectCommand> logger, 
            ISettingsProvider<LanguageServerSettings> settingsProvider,
            IProjectsProvider projectsProvider)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _projectsProvider = projectsProvider;
        }

        private TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public override void Execute(object? param)
        {
            if (TimedAction.TryRun(() =>
            {
                var lockedProjects = _projectsProvider.LockedProjects();
                var projects = _projectsProvider.Projects();



            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(TraceLevel, $"{nameof(CreateRubberduckProjectCommand)} finished executed.", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(TraceLevel, exception);
            }
        }
    }
}
