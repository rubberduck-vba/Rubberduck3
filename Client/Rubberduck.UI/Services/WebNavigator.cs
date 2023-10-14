using Rubberduck.UI.Services.Abstract;
using System;
using System.Diagnostics;

namespace Rubberduck.UI.Services
{
    public class WebNavigator : IWebNavigator
    {
        public void Navigate(Uri uri)
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
        }
    }
}
