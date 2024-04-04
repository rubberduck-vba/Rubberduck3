using Rubberduck.UI.Services.Abstract;
using System;
using System.Diagnostics;

namespace Rubberduck.UI.Services
{
    public class WebNavigator : IWebNavigator
    {
        // TODO validate that we can get a 200/OK from the specified URL before actually launching a browser process.

        public void Navigate(Uri uri)
        {
            try
            {
                Process.Start(new ProcessStartInfo(uri.AbsoluteUri));
            }
            catch
            {
                // gulp.
            }
        }
    }
}
