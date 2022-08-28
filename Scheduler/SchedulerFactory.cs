using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;


namespace AviaTickets.Scheduler
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private Queue<Action>? _scheduler;
        private ILogger<ISchedulerFactory> _logger;
        private bool _exelent = false;
        private string _procName;
        public SchedulerFactory(ILogger<ISchedulerFactory> logger)
        {
            _logger = logger;
        }

        public ISchedulerFactory Create(string procName)
        {
            Clear();
            _scheduler = new Queue<Action>();
            _procName = procName;
            return this;
        }

        public ISchedulerFactory Do(Action action)
        {
            _scheduler?.Enqueue(action);
            return this;
        }
        public bool Start()
        {
            var step = 1;

            while (_scheduler?.Count > 0)
            {
                try
                {                   
                    _scheduler?.Peek().Invoke();
                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {_procName}, STEP[{step++}] : {_scheduler?.Dequeue().Method.Name}, STATUS: {STATUS.DONE}");
                    _exelent = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[{DateTime.Now}] PROCESS : {_procName}, STEP[{step}] : {_scheduler?.Dequeue().Method.Name}, STATUS: {STATUS.ERROR} , {ex.Message}");
                    _exelent = false;
                }
            }

            Clear();
            return _exelent;
        }
        private void Clear()
        {
            _scheduler?.Clear();
        }
    }
}
