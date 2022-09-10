using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AviaTickets.Scheduler
{
    public class SchedulerFactory : ISchedulerFactory
    {
        private Queue<Action>? _scheduler;
        private ILogger<ISchedulerFactory> _logger;
        private bool _exelent = false;
        private string _procName;
        private Stopwatch _timer;
        public SchedulerFactory(ILogger<ISchedulerFactory> logger)
        {
            _logger = logger;
        }

        public ISchedulerFactory Create(string procName)
        {
            Clear();
            _scheduler = new Queue<Action>();
            _procName = procName;
            _timer = new Stopwatch();
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
                    _timer.Restart();
                    _scheduler?.Peek().Invoke();
                    _timer.Stop();
                    var ts = _timer.Elapsed;
                    var elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds,ts.Milliseconds / 10);
                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {_procName}, STEP[{step++}] : {_scheduler?.Dequeue().Method.Name}, STATUS: {STATUS.DONE}, Длительность операции : {elapsedTime} ");
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
