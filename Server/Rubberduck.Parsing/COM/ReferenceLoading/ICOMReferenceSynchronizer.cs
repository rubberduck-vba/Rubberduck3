﻿using System.Collections.Generic;
using System.Threading;

namespace Rubberduck.Parsing.COM.ReferenceLoading
{
    public interface ICOMReferenceSynchronizer
    {
        bool LastSyncOfCOMReferencesLoadedReferences { get; }
        IEnumerable<string> COMReferencesUnloadedInLastSync { get; }
        IEnumerable<(string projectId, string referencedProjectId)> COMReferencesAffectedByPriorityChangesInLastSync { get; }

        void SyncComReferences(CancellationToken token);
    }
}
