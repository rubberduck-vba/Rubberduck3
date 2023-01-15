using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC
{
    public static class RpcPort
    {
        public const int MinValue = 1024;
        public const int MaxValue = 5000;

        public static bool IsValid(int port) => port >= MinValue && port <= MaxValue;
    }
}
