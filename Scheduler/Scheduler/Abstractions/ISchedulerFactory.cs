using System;


namespace Scheduler

{
    public interface ISchedulerFactory
    {
        ISchedulerFactory Create();
        ISchedulerFactory Do(Func<IMessage, IMessage> subprocess);
        IMessage Start(IMessage msg = default);        
    }
}
