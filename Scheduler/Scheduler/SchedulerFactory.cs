using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scheduler
{
    public interface IOut
    {
        public IMessage Start(IMessage msg);
    }

    public class SchedulerFactory : ISchedulerFactory<IOut> 
    {
        private Queue<Queue<Action>> _scheduler;
        private Queue<Action> _schedulerElements;
        private Queue<IOut> _schedulerProcess;
        private IMessage _msg;
        private ILogger<ISchedulerFactory<IOut>> _logger;
        private bool _exelent = false;
        private string _procName;
        private Stopwatch _timer;
        private Exception _error;
        public SchedulerFactory(ILogger<ISchedulerFactory<IOut>> logger)
        {
            _logger = logger;            
            _scheduler = new Queue<Queue<Action>>();
        }

        public ISchedulerFactory<IOut> Create(string procName)
        {            
            _schedulerElements = new Queue<Action>();
            _procName = procName;
            _timer = new Stopwatch();
            return this;
        }

        public ISchedulerFactory<IOut> Create(IMessage msg = default)
        {
            _schedulerProcess?.Clear();
            _msg = msg;
            _schedulerProcess = new Queue<IOut>();            
            _timer = new Stopwatch();
            return this;
        }

        public ISchedulerFactory<IOut> Do(Action action)
        {
            _schedulerElements?.Enqueue(action);
            return this;
        }

        public ISchedulerFactory<IOut> Do(IOut process)
        {
            _schedulerProcess?.Enqueue(process);
            return this;
        }

        public ISchedulerFactory<IOut> Build()
        {
           if (_schedulerElements != null) _scheduler?.Enqueue(_schedulerElements);
           return this;
        }
        public void Start()
        {
            while (_schedulerProcess?.Count > 0)
            {
                if (_msg != default && !_msg.IsSuccess && !_msg.ValidateStatus) throw _msg.Error;                
                else _msg = _schedulerProcess?.Dequeue()?.Start(_msg);
            }
        }
        public (bool,Exception) StartProcess()
        {
            var step = 1;
            var item= _scheduler?.Peek();


            while (item?.Count > 0)
            {
                try
                {    
                    _timer.Restart();
                    item?.Peek().Invoke();
                    _timer.Stop();
                    var ts = _timer.Elapsed;
                    var elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds,ts.Milliseconds / 10);
                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {_procName}, STEP[{step++}] : {item?.Dequeue().Method.Name}, STATUS: {STATUS.DONE}, Длительность операции : {elapsedTime} ");
                    if(item?.Count == 0) _scheduler?.Dequeue();
                    _exelent = true;
                    _error = null;

                }
                catch (Exception ex)
                {
                    _logger.LogError($"[{DateTime.Now}] PROCESS : {_procName}, STEP[{step}] : {item?.Dequeue().Method.Name}, STATUS: {STATUS.ERROR} , {ex.Message}");
                    _schedulerProcess?.Clear();
                    _scheduler?.Clear();
                    _exelent = false;
                    _error = ex;
                }
            }           
            
            return (_exelent,_error);
        }

       
    }
}
