using AviaTickets.Abstractions;
using AviaTickets.Models;
using AviaTickets.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.Processes
{
    public class AviaTicketsGetWorkflow : IAviaTicketsGetWorkflow
    {        
        private ILogger<AviaTicketsGetWorkflow> _logger;
        private ISchedulerFactory _scheduler;
        private MainWindow _mainWindow;
        private AviaTicketsViewModel _viewModel;
        private List<TicketForm> tickets;


        private string _token;
        private string _currency;
        private List<Cities>? _cities;
        public string WorkflowType { get; set; } = "AVIA_TICKETS_GET";

        public AviaTicketsGetWorkflow(ILoggerFactory loggerFactory
            , IConfigurationRoot configuration
            , ISchedulerFactory schedulerFactory
            , MainWindow mainWindow
            , AviaTicketsViewModel viewModel)
        {           
            _logger = loggerFactory.CreateLogger<AviaTicketsGetWorkflow>();

            _token = configuration["Token"];
            _currency = configuration["Currency"];

            _mainWindow = mainWindow;

            _viewModel = viewModel;
            _viewModel.Search = new RelayCommand(obj => SearchTicket());

            _scheduler = schedulerFactory.Create()
                            .Do(() => { _cities = GetCities(); })
                            .Do(SearchTicket);
        }

        public void Start()
        {            
            _scheduler.Start();
        }

        public List<Cities>? GetCities()
        {
            var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
            request.Run(_logger);
            var response = request.Response;
            var info = JsonConvert.DeserializeObject<List<Cities>>(response);
            return info;
        }

        public async void SearchTicket()
        {
            _mainWindow.Tickets.Children.Clear();


            tickets = new List<TicketForm>();

            var depDateFormat =_viewModel.DepDate.ToString("yyyy-MM-dd");
            var arrDateFormat =_viewModel.ArrDate.ToString("yyyy-MM-dd");

            var depCityCode = GetCityCode(_viewModel.DepCity, _cities);
            var arrCityCode = GetCityCode(_viewModel.ArrCity, _cities);

            if (_viewModel.OneWayTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, default, _token, "true");
                CreateTickets(result, _viewModel.OneWayTicket, false);
            }

            if (_viewModel.ReturnTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, arrDateFormat, _token, "true");
                CreateTickets(result, false, _viewModel.ReturnTicket);
            }

            if (_viewModel.OneWayTicket && _viewModel.WayWithTransferTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, default, _token, "false");
                CreateTickets(result, _viewModel.OneWayTicket, false);
            }

            if (_viewModel.ReturnTicket && _viewModel.WayWithTransferTicket)
            {
                var result = await GetResult(depCityCode, arrCityCode, _currency, depDateFormat, arrDateFormat, _token, "false");
                CreateTickets(result, false, _viewModel.ReturnTicket);
            }

            tickets.Sort((a, b) => (a.DataContext as TicketFormViewModel).ShortPrice.CompareTo((b.DataContext as TicketFormViewModel).ShortPrice));

            for (int i = tickets.Count - 1; i >= 0; i--)
            {
                _mainWindow.Tickets.Children.Insert(0, tickets[i]);
            }
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

        public async Task<Result?> GetResult(string depCity, string arrCity, string currency, string depDate, string arrDate, string token, string direct)
        {
            var request = new GetRequest($"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={depCity}&destination={arrCity}&currency={currency}&departure_at={depDate}&return_at={arrDate}&sorting=price&direct={direct}&limit=30&token={token}");
            var response = await Task.Run(() =>
            {
                request.Run(_logger);
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

    }
}
