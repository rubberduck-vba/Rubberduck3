using System;

namespace Rubberduck.InternalApi.Services
{
    public class TimedActionFailedException : ApplicationException
    {
        public TimedActionFailedException(Exception inner)
            : base("A timed action has aborted with an exception.", inner) { }
    }
}