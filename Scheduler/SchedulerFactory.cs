using AviaTickets.Abstractions;
using AviaTickets.Processes;
using Microsoft.Extensions.Logging;
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
        private ILogger<ISchedulerFactory> _logger;
        public SchedulerFactory(ILogger<ISchedulerFactory> logger)
        {
            _logger = logger;
        }

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
            var step = 1;

            while (_scheduler?.Count > 0)
            {
                try
                {                   
                    _scheduler?.Peek().Invoke();
                    _logger.LogInformation($"STEP[{step++}] : {_scheduler?.Dequeue().Method.Name}, STATUS: {STATUS.DONE}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"STEP[{step}] : {_scheduler?.Dequeue().Method.Name}, STATUS: {STATUS.ERROR}", ex.Message);
                }
            }

            Clear();
        }
        private void Clear()
        {
            _scheduler?.Clear();
        }
    }
}
