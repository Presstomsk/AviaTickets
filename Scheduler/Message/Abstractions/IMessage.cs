using System;


namespace Scheduler
{    /// <summary>
     /// Сообщение для обмена данными между задачами
     /// </summary>
    public interface IMessage
    {   
        /// <summary>
        /// Положительный/отрицательный результат 
        /// </summary>
        bool IsSuccess { get; set; }
        /// <summary>
        /// Передаваемые данные при положительном результате
        /// </summary>
        object Data { get; set; }
        /// <summary>
        /// Тип передаваемых данных при положительном результате
        /// </summary>
        Type DataType { get; set; }
        /// <summary>
        /// Ошибка при отрицательном результате
        /// </summary>
        Exception Error { get; set; }
        /// <summary>
        /// Указание планировщику записывать ошибку только в лог, без ее генерации в коде
        /// </summary>
        bool Validate { get; set; }
    }
}
