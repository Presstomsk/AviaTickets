using AviaTickets.Controller;
using AviaTickets.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AviaTickets.ViewModel.Absractions
{
    public interface IView
    {
        event PropertyChangedEventHandler? PropertyChanged;
        event TicketClickHandler? OpenTicketLink;
        event Action? SearchTickets;        
        string DepCity { get; set; }
        string ArrCity { get; set; }
        DateTime DepDate { get; set; }
        DateTime ArrDate { get; set; }
        string TodayDate { get; set; }
        bool OneWayTicket { get; set; }
        bool ReturnTicket { get; set; }
        bool WayWithTransferTicket { get; set; }
        DateTime FirstDateStart { get; set; }
        DateTime SecondDateStart { get; set; }
        RelayCommand Search { get; }
        void OnPropertyChanged([CallerMemberName] string prop = "");
        List<ICities>? Cities { get; set; }
        List<TicketForm> Tickets { get; set; }
        void Tickets_OpenTicketLink(string link);
    }
}
