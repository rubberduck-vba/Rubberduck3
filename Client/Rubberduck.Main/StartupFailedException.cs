using System;
using System.Diagnostics;

namespace Rubberduck.Main
{
    internal class StartupFailedException : Exception
    {
        private static readonly string _message =
            "Rubberduck's startup sequence threw an unexpected exception. Please check the Rubberduck logs for more information and report an issue if necessary.";

        public StartupFailedException(Exception? inner)
            : base(_message, inner)
        { }
    }

    internal class ServerStartupFailedException : Exception
    {
        private static readonly string _message =
            "Language server was started, but the process is no longer running. Verify server logs. LSP client cannot start.";

        private readonly Process _process;

        public ServerStartupFailedException(Process process)
            : base(_message)
        {
            _process = process;

            Data.Add(nameof(Name), Name);
            Data.Add(nameof(ExitCode), ExitCode);
            Data.Add(nameof(Id), Id);
            Data.Add(nameof(StartTime), StartTime);
            Data.Add(nameof(ExitTime), ExitTime);
            Data.Add(nameof(PeakWorkingSetBytes), PeakWorkingSetBytes);
            Data.Add(nameof(WorkingSetBytes), WorkingSetBytes);
        }

        public string Name => _process.ProcessName;
        public int ExitCode => _process.ExitCode;
        public long Id => _process.Id;
        public DateTime StartTime => _process.StartTime;
        public DateTime ExitTime => _process.ExitTime;
        public long PeakWorkingSetBytes => _process.PeakWorkingSet64;
        public long WorkingSetBytes => _process.WorkingSet64;
    }
}