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
        public bool ValidateStatus { get ; set ; }

        public Message(bool isSuccess, object? data, Type? dataType, Exception? error, bool validateStatus = false)
        {
            IsSuccess = isSuccess;
            Data = data;
            DataType = dataType;
            Error = error;
            ValidateStatus = validateStatus;
        }
    }
}
