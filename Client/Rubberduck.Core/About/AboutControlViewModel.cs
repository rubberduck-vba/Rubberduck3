using System;
using System.Diagnostics;
using Path = System.IO.Path;
using Rubberduck.UI.Command;
using Application = System.Windows.Forms.Application;
using Rubberduck.VersionCheck;
using Rubberduck.Resources.About;
using System.Windows.Input;
using System.Windows;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.UI.Abstract;
using Rubberduck.UI;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Core.About
{
    public class AboutControlViewModel : ViewModelBase, IAboutControlViewModel
    {
        private readonly IMessageBox _messageBox;
        private readonly IVersionCheckService _version;
        private readonly IWebNavigator _web;

        public AboutControlViewModel(ILogger logger, IVersionCheckService version, IWebNavigator web, IMessageBox messageBox)
        {
            _messageBox = messageBox;

            _version = version;
            _web = web;

            UriCommand = new DelegateCommand(logger, ExecuteUri);
            ViewLogCommand = new DelegateCommand(logger, ExecuteViewLog);
        }

        public string Version => string.Format(Resources.RubberduckUI.Rubberduck_AboutBuild, _version.VersionString);

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

        public void CopyVersionInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(Version);
            sb.AppendLine(OperatingSystem);
            sb.AppendLine(HostProduct);
            sb.AppendLine(HostVersion);
            sb.AppendLine(HostExecutable);

            Clipboard.SetText(sb.ToString());
            _messageBox.Message(AboutUI.AboutWindow_CopyVersionMessage);
        }

        private void ExecuteUri(object parameter) => _web.Navigate(((Uri)parameter));

        private void ExecuteViewLog(object parameter)
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
