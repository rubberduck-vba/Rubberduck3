using Rubberduck.InternalApi.Model.Abstract;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.InternalApi.Model.Design
{
    public class AboutWindowDesignViewModel : IAboutWindowViewModel
    {
        public string Version => "3.0.x (debug)";

        public string OperatingSystem => "Microsoft Windows 11";

        public string HostProduct => "Microsoft Excel";

        public string HostVersion => "16.0";

        public string HostExecutable => "EXCEL.EXE";

        public string AboutCopyright => $"Copyright 2014-{DateTime.Today.Year} Rubberduck Contributors";

        public ICommand UriCommand => null!;

        public ICommand ViewLogCommand => null!;

        public string Document => "Hello, world!";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void CopyVersionInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
