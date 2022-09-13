using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scheduler
{
    /// <summary>
    /// Планировщик заданий
    /// </summary>
    public class SchedulerFactory : ISchedulerFactory
    {
        
        private Process _process;


        private ILogger<ISchedulerFactory> _logger;
        private Stopwatch _timer;
        private IMessage _msg;
        /// <summary>
        /// Конструктор планировщика
        /// </summary>
        /// <param name="logger">Логгер</param>
        public SchedulerFactory(ILogger<ISchedulerFactory> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        /// <summary>
        /// Создание списка задач
        /// </summary>
        /// <returns>Текущий планировщик заданий</returns>
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

        /// <summary>
        /// Добавление задачи в планировщик (список задач)
        /// </summary>
        /// <param name="subprocess">Задача (должна принимать и возвращать IMessage)</param>
        /// <returns>Текущий планировщик заданий</returns>
        public ISchedulerFactory Do(Func<IMessage, IMessage> subprocess)
        {
            if (_process != default)
            {
                var subProc = new Subprocess
                {
                    SubprocessName = subprocess.Method.Name,
                    Operation = subprocess
                };

                _process.Subprocesses.Enqueue(subProc);
            }

            return this;
        }

        /// <summary>
        /// Запуск выполнения очереди задач
        /// </summary>
        /// <param name="msg">Сообщение с данными для первой задачи планировщика</param>
        /// <returns>Сообщение с данными от последнего элемента планировщика</returns>
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
                            _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, STEP[{step++}] : {_process.Subprocesses.Peek().SubprocessName}, STATUS: {STATUS.DONE}, Длительность операции : {elapsedTime} ");
                            _process.Subprocesses?.Dequeue();
                        }
                        


                    }
                    catch (Exception ex)
                    {
                        _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, STEP[{step}] : {_process.Subprocesses.Peek().SubprocessName}, STATUS: {STATUS.ERROR}, {ex.Message} ");

                        _process.Subprocesses.Clear();
                        _process = default;
                        _msg = default;

                        throw ex;

                    }
                }
            }

            return _msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Сообщение, возвращаемое задачей планировщика</param>
        /// <returns>Рузультаты обработки отрицательного результата без генирации ошибки (Validate = true)</returns>
        /// <exception cref="Exception">Генерация ошибки при отрицательном результате</exception>
        private bool Validator(IMessage message)
        {          
            
            if (message != default && !message.IsSuccess && message.Validate)
            {
                _logger?.LogInformation($"[{DateTime.Now}] PROCESS : {_process.Subprocesses.Peek().Operation.Method.DeclaringType.Name}, Validator : {_process.Subprocesses.Peek().SubprocessName}, STATUS: {STATUS.ERROR}, {message.Error.Message} ");
               
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

    /// <summary>
    /// Планировщик
    /// </summary>
    public class Process 
    {   /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public Guid Guid { get; set; }        
        /// <summary>
        /// Список задач
        /// </summary>
        public Queue<Subprocess> Subprocesses { get; set; }
    }

    /// <summary>
    /// Задача
    /// </summary>
    public class Subprocess
    {   
        /// <summary>
        /// Название задачи
        /// </summary>
        public string SubprocessName { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public Func<IMessage,IMessage> Operation { get; set; }   
    }
}
