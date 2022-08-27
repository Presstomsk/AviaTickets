using System;


namespace AviaTickets.Scheduler.Abstractions
{
    public interface ISchedulerFactory
    {
        ISchedulerFactory Create();
        ISchedulerFactory Do(Action action);
        bool Start();        
    }
}
