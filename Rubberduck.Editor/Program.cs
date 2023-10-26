﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor
{
    public static class Program
    {
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            using var tokenSource = new CancellationTokenSource();

            try
            {
                var services = new ServiceCollection();
                services.AddLogging();

                var app = new App();
                return app.Run();
            }
            catch
            {
                return -1;
            }
        }


    }
}
