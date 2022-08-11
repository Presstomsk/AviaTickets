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
        


        private string _token;
        private string _currency;
        private string _depCityCode;
        private string _arrCityCode;
        private string _depDateFormat;
        private string _arrDateFormat;       

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


            _scheduler = schedulerFactory.Create()
                            .Do(ChangeDateFormat)
                            .Do(GetCitiesCodes)
                            .Do(RequestAndCreatingTickets)
                            .Do(AddTicketsToMainWindow);
        }

        public void Start()
        {
            try
            {
                _scheduler.Start();
            }
            catch (Exception ex)
            {
                _logger?.LogInformation(ex.Message, WorkflowType);
            }
        }

        public void ChangeDateFormat()
        {
            _depDateFormat = _viewModel.DepDate.ToString("yyyy-MM-dd");
            _arrDateFormat = _viewModel.ArrDate.ToString("yyyy-MM-dd");
        }

        public void GetCitiesCodes()
        {
            _depCityCode = GetCityCode(_viewModel.DepCity, _viewModel.Cities);
            _arrCityCode = GetCityCode(_viewModel.ArrCity, _viewModel.Cities);
        }

        public void RequestAndCreatingTickets()
        {
            _viewModel.Tickets = new List<TicketForm>();

            if (_viewModel.OneWayTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "true");
                CreateTickets(result, _viewModel.OneWayTicket, false);
            }
            if (_viewModel.ReturnTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "true");
                CreateTickets(result, false, _viewModel.ReturnTicket);
            }

            if (_viewModel.OneWayTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "false");
                CreateTickets(result, _viewModel.OneWayTicket, false);
            }

            if (_viewModel.ReturnTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "false");
                CreateTickets(result, false, _viewModel.ReturnTicket);
            }

            _viewModel.Tickets.Sort((a, b) => (a.DataContext as TicketFormViewModel).ShortPrice.CompareTo((b.DataContext as TicketFormViewModel).ShortPrice));
        }

        public void AddTicketsToMainWindow()
        {
            _mainWindow.Tickets.Children.Clear();          

            for (int i = _viewModel.Tickets.Count - 1; i >= 0; i--)
            {
                _mainWindow.Tickets.Children.Insert(0, _viewModel.Tickets[i]);
            }
        }

        public string GetCityCode(string mycity, List<Cities>? cities)
        {
            if (cities != null)
            {
                foreach (var city in cities)
                {
                    if (city.City == mycity) return city.Code;
                    if (city.Code == mycity) return city.Code;
                }
            }
            return String.Empty;
        }

        public Result? GetResult(string depCity, string arrCity, string currency, string depDate, string arrDate, string token, string direct)
        {
            var request = new GetRequest($"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={depCity}&destination={arrCity}&currency={currency}&departure_at={depDate}&return_at={arrDate}&sorting=price&direct={direct}&limit=30&token={token}");
            request.Run();
            var response = request.Response;
            var info = JsonConvert.DeserializeObject<Result>(response);
            return info;            
        }

        public void CreateTickets(Result? info, bool oneWayTicket, bool returnTicket)
        {
            if (info != null) info.Data.ForEach(item =>
            {
                var ticketForm = new TicketForm();
                var ticket = (ticketForm.DataContext != null) ? ticketForm.DataContext as TicketFormViewModel : default;
                if (ticket != default)
                {
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

                    _viewModel.Tickets.Add(ticketForm);
                }
            });         
        }
    }
}
