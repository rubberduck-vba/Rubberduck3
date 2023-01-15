using System;

namespace Rubberduck.DataServer.Services
{
    public interface IEnvironmentService
    {
        void Exit(int code = 0);
    }
    public class EnvironmentService : IEnvironmentService
    {
        public void Exit(int code = 0)
        {
            Environment.Exit(code);
        }
    }
}
