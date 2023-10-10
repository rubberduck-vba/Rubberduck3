using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Message;
using Rubberduck.VersionCheck;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command
{
    public class VersionCheckCommand : CommandBase
    {
        private readonly IVersionCheckService _service;
        private readonly IMessageService _prompt;
        //private readonly IWebNavigator _webNavigator;

        public VersionCheckCommand(ILogger<VersionCheckCommand> logger, ISettingsProvider<RubberduckSettings> settings, 
            IVersionCheckService service, IMessageService prompt /*, IWebNavigator web*/ )
            : base(logger, settings)
        {
            _service = service;
            _prompt = prompt;
            //_webNavigator = web;
        }

        protected override async Task OnExecuteAsync(object? parameter)
        {
            var settings = SettingsProvider.Settings.UpdateServerSettings;
            Logger.LogInformation("Executing version check...");

            var latest = await _service.GetLatestVersionAsync(CancellationToken.None);

            if (_service.CurrentVersion < latest)
            {
                var proceed = true;
                if (_service.IsDebugBuild || !settings.IncludePreReleases)
                {
                    // if the latest version has a revision number and isn't a pre-release build,
                    // avoid prompting since we can't know if the build already includes the latest version.
                    proceed = latest.Revision == 0;
                }

                if (proceed)
                {
                    //PromptAndBrowse(t.Result, settings.IncludePreRelease);
                }
            }
        }

        //private void PromptAndBrowse(Version latestVersion, bool includePreRelease)
        //{
        //    //var buildType = includePreRelease 
        //    //    ? RubberduckUI.VersionCheck_BuildType_PreRelease 
        //    //    : RubberduckUI.VersionCheck_BuildType_Release;
        //    //var prompt = string.Format(RubberduckUI.VersionCheck_NewVersionAvailable, _versionCheck.CurrentVersion, latestVersion, buildType);
        //    //if (!_prompt.Question(prompt, RubberduckUI.Rubberduck))
        //    //{
        //    //    return;
        //    //}

        //    //var url = new Uri(includePreRelease
        //    //    ? "https://github.com/rubberduck-vba/Rubberduck/releases"
        //    //    : "https://github.com/rubberduck-vba/Rubberduck/releases/latest");
        //    //_webNavigator.Navigate(url);
        //}
    }
}
