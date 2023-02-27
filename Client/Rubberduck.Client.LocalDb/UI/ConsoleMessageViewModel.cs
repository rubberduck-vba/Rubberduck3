using Rubberduck.InternalApi.Common;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;

namespace Rubberduck.Client.LocalDb
{
    public class ConsoleMessageViewModel : ViewModelBase, IConsoleMesssageViewModel, IExportable
    {
        public ConsoleMessageViewModel(ConsoleMessage args)
        {
            Id = args.Id;
            Timestamp = args.Timestamp;
            IsError = args.IsError;
            Exception = args.Exception;
            Level = (int)args.Level;
            Message = args.Message;
            Verbose = args.Verbose;
        }

        public int Id { get; }
        public DateTime Timestamp { get; }
        public bool IsError { get; }
        public bool IsWarning => Level == (int)ServerLogLevel.Warn;
        public Exception Exception { get; }
        public int Level { get; }
        public string Message { get; }
        public string Verbose { get; }

        public object[] ToArray()
        {
            return new object[]
            {
                Id,
                Timestamp,
                IsError,
                Exception,
                Level,
                Message,
                Verbose
            };
        }

        public string ToClipboardString()
        {
            return string.Join("\t", ToArray());
        }
    }
}
