using AviaTickets.Converters;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AviaTickets.Processes
{
    public class AviaTicketsGetWorkflow : IAviaTicketsGetWorkflow
    {   
        private ISchedulerFactory<IOut> _scheduler;        
        private IView _viewModel;       
        private TicketConverter _converter;
        private List<Data> _data;
        

        private string _token;
        private string _currency;
        private string _depCityCode;
        private string _arrCityCode;
        private string _depDateFormat;
        private string _arrDateFormat;       

        public string WorkflowType { get; set; } = "AVIA_TICKETS_GET_WORKFLOW";

        public AviaTicketsGetWorkflow(IConfigurationRoot configuration
                                     , ISchedulerFactory<IOut> schedulerFactory            
                                     , IView viewModel            
                                     , TicketConverter converter)
        {           

            _token = configuration["Token"];
            _currency = configuration["Currency"];
           
            _viewModel = viewModel;            
            _converter = converter;

            _scheduler = schedulerFactory.Create(WorkflowType)
                            .Do(ChangeDateFormat)
                            .Do(GetCitiesCodes)
                            .Do(RequestTickets)
                            .Build();
                            
        }

        public IMessage? Start(IMessage? msg)
        {
            if (msg != default)
            {
                if (msg.IsSuccess)
                {
                    return Start();
                }
                else
                {
                    throw msg.Error ?? new Exception();
                }
            }
            return Start();
        }

        public IMessage? Start()
        {
            var answer = _scheduler.StartProcess();
            return new Msg.Message(answer.Item1, _data, _data.GetType(), answer.Item2);
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
            var tickets = new List<ITicket>();

            if (_viewModel.OneWayTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "true");
                if (result != default) tickets.Add(result);                
            }
            if (_viewModel.ReturnTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "true");
                if (result != default) tickets.Add(result);                
            }

            if (_viewModel.OneWayTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, "", _token, "false");
                if (result != default) tickets.Add(result);                
            }

            if (_viewModel.ReturnTicket && _viewModel.WayWithTransferTicket)
            {
                var result = GetResult(_depCityCode, _arrCityCode, _currency, _depDateFormat, _arrDateFormat, _token, "false");
                if (result != default) tickets.Add(result);               
            }
            List<Data> ticketData = new List<Data>();
            tickets.ForEach(x => { x.Data.ForEach(x => { ticketData.Add(x); }); });
            List<Data> distinct = ticketData.Distinct().ToList();

            _data = distinct;
            _data.Sort((a, b) => a.Price.CompareTo(b.Price));
        }       

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
