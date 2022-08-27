using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AviaTickets.Controller;
using AviaTickets.Models.Abstractions;
using AviaTickets.ViewModel.Absractions;

namespace AviaTickets.ViewModel
{
    public class View : IView , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event TicketClickHandler? OpenTicketLink;
        public event Action? SearchTickets;       

        private string _depCity;
        private string _arrCity;
        private DateTime _depDate = DateTime.Now;
        private DateTime _arrDate = DateTime.Now;
        private string _todayDate;        
        private bool _oneWayTicket = false;
        private bool _returnTicket = false;
        private bool _wayWithTransferTicket = false;
        private DateTime _firstDateStart = DateTime.Now;
        private DateTime _secondDateStart = DateTime.Now;       

        private RelayCommand _search;      

        public List<ICities>? Cities { get; set; }
        public List<TicketForm> Tickets { get; set; }

        public string DepCity
        {
            get 
            {
                return _depCity;
            }
            set
            {
                _depCity = value;
                OnPropertyChanged(nameof(DepCity));
            }        
        }

        public string ArrCity
        {
            get
            {
                return _arrCity;
            }
            set
            {
                _arrCity = value;
                OnPropertyChanged(nameof(ArrCity));
            }
        }

        public DateTime DepDate
        {
            get
            {
                return _depDate;
            }
            set
            {
                _depDate = value;
                SecondDateStart = _depDate;
                ArrDate = _depDate;
                OnPropertyChanged(nameof(DepDate));
            }
        }

        public DateTime ArrDate
        {
            get
            {
                return _arrDate;
            }
            set
            {
                _arrDate = value;
                OnPropertyChanged(nameof(ArrDate));
            }
        }

        public string TodayDate
        {
            get
            {
                return _todayDate;
            }
            set
            {
                _todayDate = value;
                OnPropertyChanged(nameof(TodayDate));
            }
        }
        public bool OneWayTicket
        {
            get
            {
                return _oneWayTicket;
            }
            set
            {
                _oneWayTicket = value;
                OnPropertyChanged(nameof(OneWayTicket));
            }
        }

        public bool ReturnTicket
        {
            get
            {
                return _returnTicket;
            }
            set
            {
                _returnTicket = value;
                OnPropertyChanged(nameof(ReturnTicket));
            }
        }
        
        public bool WayWithTransferTicket
        {
            get
            {
                return _wayWithTransferTicket;
            }
            set
            {
                _wayWithTransferTicket = value;
                OnPropertyChanged(nameof(WayWithTransferTicket));
            }
        }

        public DateTime FirstDateStart
        {
            get
            {
                return _firstDateStart;
            }
            set
            {
                _firstDateStart = value;
                OnPropertyChanged(nameof(FirstDateStart));
            }
        }

        public DateTime SecondDateStart
        {
            get
            {
                return _secondDateStart;
            }
            set
            {
                _secondDateStart = value;
                OnPropertyChanged(nameof(SecondDateStart));
            }
        }

        public RelayCommand Search
        {
            get
            {
                return _search ?? (_search = new RelayCommand(obj => Find()));
            }
        } 
        public void Find()
        {
            SearchTickets?.Invoke();      
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Tickets_OpenTicketLink(string link)
        {
            OpenTicketLink?.Invoke(link);
        }

        
    }
   
}
