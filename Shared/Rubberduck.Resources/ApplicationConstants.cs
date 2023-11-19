using System;
using Path = System.IO.Path;

namespace Rubberduck.Resources
{
    public static class ApplicationConstants
    {
        /// <summary>
        /// The root <c>Rubberduck</c> folder directly under the current user's <c>%appdata%</c> directoty.
        /// </summary>
        public static readonly string RUBBERDUCK_FOLDER_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck");

        /// <summary>
        /// The path for log files, located under the <c>RUBBERDUCK_FOLDER_PATH</c>.
        /// </summary>
        public static readonly string LOG_FOLDER_PATH = Path.Combine(RUBBERDUCK_FOLDER_PATH, "Logs");

        /// <summary>
        /// The path for project templates, located under the <c>RUBBERDUCK_FOLDER_PATH</c>.
        /// </summary>
        public static readonly string TEMPLATES_FOLDER_PATH = Path.Combine(RUBBERDUCK_FOLDER_PATH, "Templates");

        /// <summary>
        /// The root <c>Rubberduck</c> folder directly under the current user's <c>%temp%</c> directory.
        /// </summary>
        public static readonly string RUBBERDUCK_TEMP_PATH = Path.Combine(Path.GetTempPath(), "Rubberduck");
    }
}
