

using AviaTickets.Models.Abstractions;
using System;
using System.Collections.Generic;

namespace AviaTickets.Processes.Abstractions
{
    public interface ICitiesDatabaseUpdateWorkflow : IWorkflow
    {
        DateTime? UpdateDate { get; }
        bool IsNeedUpdate { get; }
        List<ICities>? Info { get; }
    }
}
