using AviaTickets.Abstractions;
using AviaTickets.Сontrol;
using AviaTickets.Processes;
using AviaTickets.Scheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AviaTickets.ViewModel
{
    public class AviaTicketsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;     

       
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
            get {  return _search; }
            set { _search = value; }
        }   


        public AviaTicketsViewModel(MainWindow mainWindow)       {
            

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            
            var serilog = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilog));            

            var serviceProvider = new ServiceCollection()
                                    .AddSingleton<MainWindow>(mainWindow)
                                    .AddSingleton<AviaTicketsViewModel>(this)
                                    .AddSingleton<IConfigurationRoot>(configuration)
                                    .AddSingleton<ILoggerFactory>(loggerFactory)
                                    .AddSingleton<ISchedulerFactory,SchedulerFactory>()
                                    .AddSingleton<IAviaTicketsGetWorkflow,AviaTicketsGetWorkflow>()
                                    .BuildServiceProvider();
            Dispatcher.Start(serviceProvider);

        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
