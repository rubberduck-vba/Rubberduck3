using System;

namespace Rubberduck.Editor.Services.Abstract
{
    public interface IWebNavigator
    {
        /// <summary>
        /// Opens the specified URI in the default browser.
        /// </summary>
        void Navigate(Uri uri);
    }
}
