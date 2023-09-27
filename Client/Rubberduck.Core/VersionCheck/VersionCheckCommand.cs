using System;
using Rubberduck.VersionCheck;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.Settings;
using Rubberduck.Interaction.MessageBox;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using System.Threading;

namespace Rubberduck.UI.Command
{
    public class VersionCheckCommand : CommandBase
    {
        private readonly IVersionCheckService _versionCheck;
        //private readonly IMessageBox _prompt;
        //private readonly IWebNavigator _webNavigator;
        private readonly ISettingsProvider<UpdateServerSettings> _config;

        public VersionCheckCommand(IVersionCheckService versionCheck, /*IMessageBox prompt, IWebNavigator web,*/ ISettingsProvider<UpdateServerSettings> config)
        {
            _versionCheck = versionCheck;
            //_prompt = prompt;
            //_webNavigator = web;
            _config = config;
        }

        protected override async Task OnExecuteAsync(object parameter)
        {
            var config = _config.Value;
            Logger.LogInformation("Executing version check...");

            var latest = await _versionCheck.GetLatestVersionAsync(CancellationToken.None);

            if (_versionCheck.CurrentVersion < latest)
            {
                var proceed = true;
                if (_versionCheck.IsDebugBuild || !config.Settings.IncludePreReleases)
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
