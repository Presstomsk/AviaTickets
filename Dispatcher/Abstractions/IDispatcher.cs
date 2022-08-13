using AviaTickets.Processes.AllProcessesList;
using System;


namespace AviaTickets.Dispatcher.Abstractions
{
    public interface IDispatcher
    {
        void Start(IServiceProvider serviceProvider, ProcessType process);
    }
}
