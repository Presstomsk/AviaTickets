using AviaTickets.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Scheduler
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private Queue<Action>? _scheduler;
        public ISchedulerFactory Create()
        {
            Clear();
           _scheduler = new Queue<Action>();
            return this;
        }

        public ISchedulerFactory Do(Action action)
        {
            _scheduler?.Enqueue(action);
            return this;
        }
        public void Start()
        {            
            while (_scheduler?.Count > 0)
            { 
               _scheduler?.Dequeue().Invoke();               
            }

            Clear();
        }
        private void Clear()
        {
            _scheduler?.Clear();
        }
    }
}
