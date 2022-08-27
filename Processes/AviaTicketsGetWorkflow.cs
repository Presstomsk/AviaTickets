﻿using AviaTickets.Converters;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using AviaTickets.ViewModel;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AviaTickets.Processes
{
    public class AviaTicketsGetWorkflow : IAviaTicketsGetWorkflow
    {        
        private ILogger<AviaTicketsGetWorkflow> _logger;
        private ISchedulerFactory _scheduler;
        private MainWindow _mainWindow;
        private IView _viewModel;       
        private TicketConverter _converter;
        private List<Data> _data;
        

        private string _token;
        private string _currency;
        private string _depCityCode;
        private string _arrCityCode;
        private string _depDateFormat;
        private string _arrDateFormat;       

        public string WorkflowType { get; set; } = "AVIA_TICKETS_GET";

        public AviaTicketsGetWorkflow(ILogger<AviaTicketsGetWorkflow> logger
            , IConfigurationRoot configuration
            , ISchedulerFactory schedulerFactory
            , MainWindow mainWindow
            , IView viewModel            
            , TicketConverter converter)
        {
            _logger = logger;

            _token = configuration["Token"];
            _currency = configuration["Currency"];

            _mainWindow = mainWindow;
            _viewModel = viewModel;            
            _converter = converter;



            _scheduler = schedulerFactory.Create()
                            .Do(ChangeDateFormat)
                            .Do(GetCitiesCodes)
                            .Do(RequestTickets);
                            
        }

        public (bool, object?) Start()
        {
            var result = _scheduler.Start();
            return (result, _data);
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

        public void RequestTickets()
        {
            //_viewModel.Tickets = new List<TicketForm>();
            var tickets = new List<ITicket>();

            if (_viewModel.OneWayTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "true");
                if (result != default) tickets.Add(result);
                //CreateTickets(result, _viewModel.OneWayTicket, false);
            }
            if (_viewModel.ReturnTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "true");
                if (result != default) tickets.Add(result);
                //CreateTickets(result, false, _viewModel.ReturnTicket);
            }

            if (_viewModel.OneWayTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "false");
                if (result != default) tickets.Add(result);
                //CreateTickets(result, _viewModel.OneWayTicket, false);
            }

            if (_viewModel.ReturnTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "false");
                if (result != default) tickets.Add(result);
                //CreateTickets(result, false, _viewModel.ReturnTicket);
            }
            List<Data> ticketData = new List<Data>();
            tickets.ForEach(x => { x.Data.ForEach(x => { ticketData.Add(x); }); });
            List<Data> distinct = ticketData.Distinct().ToList();

            _data = distinct;
            //_viewModel.Tickets.Sort((a, b) => (a.DataContext as Tickets).ShortPrice.CompareTo((b.DataContext as Tickets).ShortPrice));
        }

       /* public void AddTicketsToMainWindow()
        {
            _mainWindow.Tickets.Children.Clear();          

            for (int i = _viewModel.Tickets.Count - 1; i >= 0; i--)
            {
                _mainWindow.Tickets.Children.Insert(0, _viewModel.Tickets[i]);
            }
        }*/

        public string GetCityCode(string mycity, List<ICities>? cities)
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

        public ITicket? GetResult(string depCity, string arrCity, string currency, string depDate, string arrDate, string token, string direct)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);
            var request = new GetRequest($"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={depCity}&destination={arrCity}&currency={currency}&departure_at={depDate}&return_at={arrDate}&sorting=price&direct={direct}&limit=30&token={token}");
            request.Run();
            var response = request.Response;
            var info = JsonConvert.DeserializeObject<ITicket>(response , settings);
            return info;            
        }
       
    }
}
