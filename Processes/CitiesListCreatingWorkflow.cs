using AviaTickets.Abstractions;
using AviaTickets.Converters;
using AviaTickets.Models;
using AviaTickets.ViewModel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace AviaTickets.Processes
{
    public class CitiesListCreatingWorkflow : ICitiesListCreatingWorkflow
    {
        private ILogger<CitiesListCreatingWorkflow> _logger;
        private ISchedulerFactory _scheduler;        
        private AviaTicketsViewModel _viewModel;        
        private CitiesConverter _converter;
        public string WorkflowType { get; set; } = "CITIES_LIST_CREATING";        

        public CitiesListCreatingWorkflow(ILogger<CitiesListCreatingWorkflow> logger
            , ISchedulerFactory schedulerFactory            
            , AviaTicketsViewModel viewModel            
            , CitiesConverter converter)
        {
            _logger = logger;  
            _viewModel = viewModel;
            _converter = converter;

            _scheduler = schedulerFactory.Create()
                                         .Do(GetCities);
                            
        }      

        public void Start()
        {
            try
            {
                _logger.LogInformation($"PROCESS: {WorkflowType} STATUS: {STATUS.START}");
                _scheduler.Start();
                _logger.LogInformation($"PROCESS: {WorkflowType} STATUS: {STATUS.DONE}");
            }
            catch (Exception ex)
            {                
                _logger?.LogError($"PROCESS: {WorkflowType} STATUS: {STATUS.ERROR}", ex.Message);
            }

        }

        public void GetCities()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);

            var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
            request.Run();
            var response = request.Response;
            var info = JsonConvert.DeserializeObject<List<ICities>>(response, settings);            
            _viewModel.Cities = info;
        }
    }
}
