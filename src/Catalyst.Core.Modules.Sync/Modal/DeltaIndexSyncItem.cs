using System;
using System.Collections.Generic;
using Catalyst.Protocol.IPPN;

namespace Catalyst.Core.Modules.Sync.Modal
{
    public class DeltaIndexSyncItem
    {
        public DeltaHistoryRequest Request { set; get; }
        public List<DeltaIndexScore> DeltaIndexRangeRanked { set; get; }
        public DateTime LastUpdated { set; get; }
        public DeltaIndexSyncItem() { LastUpdated = DateTime.UtcNow; }
    }
}
