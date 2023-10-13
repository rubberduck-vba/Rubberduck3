using Rubberduck.Editor.Services.Abstract;
using System;
using System.Diagnostics;

namespace Rubberduck.Editor.Services
{
    public class WebNavigator : IWebNavigator
    {
        public void Navigate(Uri uri)
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
        }
    }
}
