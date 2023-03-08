using System;
using System.Threading.Tasks;

namespace Rubberduck.Server
{
    public static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            /*TODO accept command-line arguments*/
            //await StartAsync();

            return 0;
        }
    }
}
