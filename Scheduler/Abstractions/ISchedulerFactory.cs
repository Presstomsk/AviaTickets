using System;


namespace AviaTickets.Scheduler.Abstractions
{
    public interface ISchedulerFactory
    {
        ISchedulerFactory Create(string name);
        ISchedulerFactory Do(Action action);
        bool Start();        
    }
}
