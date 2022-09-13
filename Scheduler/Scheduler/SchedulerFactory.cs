using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scheduler
{

    public class SchedulerFactory : ISchedulerFactory
    {

        private Process _process;


        private ILogger<ISchedulerFactory> _logger;
        private Stopwatch _timer;
        private IMessage _msg;

        public SchedulerFactory(ILogger<ISchedulerFactory> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public ISchedulerFactory Create()
        {
            _process = new Process
            {
                Guid = Guid.NewGuid()
                                   ,
                Subprocesses = new Queue<Subprocess>()
            };

            return this;
        }

        public ISchedulerFactory Do(Func<IMessage, IMessage> subprocess)
        {
            if (_process != default)
            {
                var subProc = new Subprocess
                {
                    SubprocessName = subprocess.ToString(),
                    Operation = subprocess
                };

                _process.Subprocesses.Enqueue(subProc);
            }

            return this;
        }

        public IMessage Start(IMessage msg = default)
        {
            _msg = msg;

            if (_process != default)
            {
                var step = 1;

                while (_process?.Subprocesses.Count > 0)
                {

                    try
                    {
                        _timer.Restart();

                        _msg = _process.Subprocesses.Peek().Operation.Invoke(_msg);

                        if(Validator(_msg))
                        {
                            return _msg;
                        }

                        _timer.Stop();

                        var ts = _timer.Elapsed;
                        var elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                        if (_process != default)
                        {
                            _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, STEP[{step++}] : {_process.Subprocesses.Peek().Operation.Method.Name}, STATUS: {STATUS.DONE}, Длительность операции : {elapsedTime} ");
                            _process.Subprocesses?.Dequeue();
                        }
                        


                    }
                    catch (Exception ex)
                    {
                        _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, STEP[{step}] : {_process.Subprocesses.Peek().Operation.Method.Name}, STATUS: {STATUS.ERROR}, {ex.Message} ");

                        _process.Subprocesses.Clear();
                        _process = default;
                        _msg = default;

                        throw ex;

                    }
                }
            }

            return _msg;
        }

        private bool Validator(IMessage message)
        {          
            
            if (message != default && !message.IsSuccess && message.Validate)
            {
                _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, Validator : {_process.Subprocesses.Peek().Operation.Method.Name}, STATUS: {STATUS.ERROR}, {message.Error.Message} ");
               
                _process.Subprocesses.Clear();
                _process = default;

                return true;
            }
            else if (message != default && !message.IsSuccess && !message.Validate)
            {
                throw new Exception(message.Error?.Message);
            }

            return false;
        }
    }
    public class Process 
    {
        public Guid Guid { get; set; }          
        public Queue<Subprocess> Subprocesses { get; set; }
    }
    public class Subprocess
    {
        public string SubprocessName { get; set; }
        public Func<IMessage,IMessage> Operation { get; set; }   
    }
}
