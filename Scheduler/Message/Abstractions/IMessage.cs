using System;


namespace Scheduler
{
    public interface IMessage
    {
        bool IsSuccess { get; set; }
        object Data { get; set; }
        Type DataType { get; set; }
        Exception Error { get; set; }
        bool ValidateStatus { get; set; }
    }
}
