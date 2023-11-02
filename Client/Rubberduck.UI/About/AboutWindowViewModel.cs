using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources.About;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Forms.Application;
using Path = System.IO.Path;

namespace Rubberduck.UI.About
{
    public class AboutWindowViewModel : ViewModelBase, IAboutWindowViewModel
    {
        //private readonly IMessageBox _messageBox;
        private readonly IWebNavigator _web;
        private readonly Version _version;

        public AboutWindowViewModel() { /* designer ctor */ }

        public AboutWindowViewModel(ILogger<AboutWindowViewModel> logger, ISettingsProvider<RubberduckSettings> settings, IWebNavigator web/*, IMessageBox messageBox*/, Version version)
        {
            //_messageBox = messageBox;

            _version = version;
            _web = web;

            Document = "TODO: load about.md";

            UriCommand = new DelegateCommand(logger, settings, ExecuteUri);
            ViewLogCommand = new DelegateCommand(logger, settings, ExecuteViewLog);
        }

        public string Version => string.Format(Resources.RubberduckUI.Rubberduck_AboutBuild, $"v{_version.ToString(3)}");

        public string OperatingSystem =>
            string.Format(AboutUI.AboutWindow_OperatingSystem, Environment.OSVersion.VersionString, Environment.Is64BitOperatingSystem ? "x64" : "x86");

        public string HostProduct =>
            string.Format(AboutUI.AboutWindow_HostProduct, Application.ProductName, Environment.Is64BitProcess ? "x64" : "x86");

        public string HostVersion => string.Format(AboutUI.AboutWindow_HostVersion, Application.ProductVersion);

        public string HostExecutable => string.Format(AboutUI.AboutWindow_HostExecutable,
            Path.GetFileName(Application.ExecutablePath).ToUpper()); // .ToUpper() used to convert ExceL.EXE -> EXCEL.EXE

        public string AboutCopyright =>
            string.Format(AboutUI.AboutWindow_Copyright, DateTime.Now.Year);

        public ICommand UriCommand { get; }

        public ICommand ViewLogCommand { get; }

        public string Document { get; }

        public void CopyVersionInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(Version);
            sb.AppendLine(OperatingSystem);
            sb.AppendLine(HostProduct);
            sb.AppendLine(HostVersion);
            sb.AppendLine(HostExecutable);

            Clipboard.SetText(sb.ToString());
            //_messageBox.Message(AboutUI.AboutWindow_CopyVersionMessage);
        }

        private void ExecuteUri(object? parameter) => _web.Navigate((Uri)(parameter ?? throw new ArgumentNullException(nameof(parameter))));

        private void ExecuteViewLog(object? parameter)
        {
            //var fileTarget = (FileTarget) LogManager.Configuration.FindTargetByName("file");

            //var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now }; 
            //var fileName = fileTarget.FileName.Render(logEventInfo);

            //// The /select argument will only work if the path has backslashes
            //fileName = fileName.Replace("/", "\\");
            //Process.Start(new ProcessStartInfo("explorer.exe", $"/select, \"{fileName}\""));
        }
    }
}
