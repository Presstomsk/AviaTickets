using Scheduler;
using System;

namespace AviaTickets.Processes.Msg
{
    public class Message : IMessage
    {
        public bool IsSuccess { get ; set; }
        public object? Data { get ; set ; }
        public Type? DataType { get ; set; }
        public Exception? Error { get ; set; }
        public bool Validate { get ; set ; }

        public Message(object? data, Type? dataType, bool isSuccess = true, Exception? error = default, bool validate = false)
        {
            IsSuccess = isSuccess;
            Data = data;
            DataType = dataType;
            Error = error;
            Validate = validate;
        }
    }
}
