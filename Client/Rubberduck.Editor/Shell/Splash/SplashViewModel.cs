﻿using Rubberduck.UI;
using Rubberduck.UI.Shell.Splash;
using System;

namespace Rubberduck.Editor.Shell.Splash
{
    public class SplashViewModel : ViewModelBase, ISplashViewModel
    {
        public SplashViewModel(Version version)
        {
            Title = Resources.RubberduckUI.Rubberduck;
            Version = version.ToString(3);
        }

        public string Title { get; }

        public string Version { get; }

        private string _status = "long live the cucumber!";
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public void UpdateStatus(string status)
        {
            Status = status;
        }
    }
}
