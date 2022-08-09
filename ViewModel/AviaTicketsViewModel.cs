using AviaTickets.Models;
using Microsoft.Extensions.Configuration;
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

        private MainWindow _mainWindow;

        private List<TicketForm> tickets;
        private List<Cities>? _cities;

        public ILogger<AviaTicketsViewModel> logger;

        

        private string _token;
        private string _currency;
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
            get
            {
                return _search ?? (_search = new RelayCommand(obj=>SearchTicket())); 
            }
        }   


        public  AviaTicketsViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _token = configuration["Token"];
            _currency = configuration["Currency"];

            _cities = GetCities();

            var serilog = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilog));
            logger = loggerFactory.CreateLogger<AviaTicketsViewModel>();

        }

        

        public string GetCityCode(string mycity, List<Cities>? cities)
        {
            foreach (var city in cities)
            {
                if (city.City == mycity) return city.Code;
                if (city.Code == mycity) return city.Code;
            }
            return null;
        }

        public async void SearchTicket()
        {
            _mainWindow.Tickets.Children.Clear();
            

            tickets = new List<TicketForm>();

            var depDateFormat = DepDate.ToString("yyyy-MM-dd");
            var arrDateFormat = ArrDate.ToString("yyyy-MM-dd");

            var depCityCode = GetCityCode(DepCity,_cities);
            var arrCityCode = GetCityCode(ArrCity,_cities);

            if (_oneWayTicket )
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, default, _token, "true");
                CreateTickets(result, _oneWayTicket, false);
            }

            if (_returnTicket )
            {               
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, arrDateFormat, _token, "true");
                CreateTickets(result, false, _returnTicket);
            }

            if (_oneWayTicket && _wayWithTransferTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, default, _token, "false");
                CreateTickets(result, _oneWayTicket, false);
            }

            if (_returnTicket && _wayWithTransferTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, arrDateFormat, _token, "false");
                CreateTickets(result, false, _returnTicket);
            }

            tickets.Sort((a, b) => (a.DataContext as TicketFormViewModel).ShortPrice.CompareTo((b.DataContext as TicketFormViewModel).ShortPrice));

            for (int i = tickets.Count - 1; i >= 0; i--)
            {                
                _mainWindow.Tickets.Children.Insert(0, tickets[i]);
            }          
        }        

        public List<Cities>? GetCities()
        {
            var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
            request.Run(logger);
            var response = request.Response;            
            var info = JsonConvert.DeserializeObject<List<Cities>>(response);
            return info;
        }

        public async Task<Result?> GetResult(string depCity, string arrCity, string currency, string depDate, string arrDate, string token, string direct )
        {
            var request = new GetRequest($"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={depCity}&destination={arrCity}&currency={currency}&departure_at={depDate}&return_at={arrDate}&sorting=price&direct={direct}&limit=30&token={token}");
            var response = await Task.Run(() =>
            {
                request.Run(logger);
                var response = request.Response;
                return response;
            });
            var info = await Task.Run(() => JsonConvert.DeserializeObject<Result>(response));
            return info;
        }

        public void CreateTickets(Result? info, bool oneWayTicket, bool returnTicket)
        {
            foreach (var item in info.Data)
            {
                var ticketForm = new TicketForm();
                var ticket = ticketForm.DataContext as TicketFormViewModel;
                ticket.Link = item.Link;
                ticket.DepCity = item.Origin;
                ticket.ArrCity = item.Destination;
                if (oneWayTicket) ticket.SearchingMethod = "OneWayTicket";
                if (returnTicket) ticket.SearchingMethod = "ReturnTicket";
                ticket.Time = $"{ item.Duration / 60}ч. { item.Duration % 60}мин.";
                ticket.Transfer = $"Кол-во пересадок: {item.Transfers}";
                ticket.ShortPrice = item.Price;
                ticket.Price = $"{ticket.ShortPrice} {info.Currency}";
                ticket.Company = $"{item.Airline}\n{item.FlightNumber}";
                if (oneWayTicket) ticket.Pic = "Resources/OneWayStrelka.jpg";
                if (returnTicket) ticket.Pic = "Resources/ReturnWay.jpg";

                tickets.Add(ticketForm);
            }
        }


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
