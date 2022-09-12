using System;


namespace Scheduler

{
    public interface ISchedulerFactory<T> where T : class
    {
        ISchedulerFactory<T> Create(IMessage msg = default);
        ISchedulerFactory<T> Create(string name);        
        ISchedulerFactory<T> Do(Action action);
        ISchedulerFactory<T> Do(T process);
        ISchedulerFactory<T> Build();
        (bool,Exception) StartProcess();
        void Start();
    }
}
