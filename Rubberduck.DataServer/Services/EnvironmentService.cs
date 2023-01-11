using System;

namespace Rubberduck.DataServer.Services
{
    internal interface IEnvironmentService
    {
        void Exit(int code = 0);
    }
    internal class EnvironmentService : IEnvironmentService
    {
        public void Exit(int code = 0)
        {
            Environment.Exit(code);
        }
    }
}
