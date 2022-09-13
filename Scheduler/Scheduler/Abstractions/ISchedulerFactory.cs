using System;


namespace Scheduler

{   /// <summary>
    /// Планировщик заданий
    /// </summary>
    public interface ISchedulerFactory
    {
        /// <summary>
        /// Создание списка задач
        /// </summary>
        /// <returns>Текущий планировщик заданий</returns>
        ISchedulerFactory Create();
        /// <summary>
        /// Добавление задачи в планировщик (список задач)
        /// </summary>
        /// <param name="subprocess">Задача (должна принимать и возвращать IMessage)</param>
        /// <returns>Текущий планировщик заданий</returns>
        ISchedulerFactory Do(Func<IMessage, IMessage> subprocess);
        /// <summary>
        /// Запуск выполнения очереди задач
        /// </summary>
        /// <param name="msg">Сообщение с данными для первой задачи планировщика</param>
        /// <returns>Сообщение с данными от последнего элемента планировщика</returns>
        IMessage Start(IMessage msg = default);        
    }
}
