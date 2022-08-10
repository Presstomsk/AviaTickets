using AviaTickets.Abstractions;
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
        public string WorkflowType { get; set; } = "CITIES_LIST_CREATING";        

        public CitiesListCreatingWorkflow(ILoggerFactory loggerFactory
            , ISchedulerFactory schedulerFactory            
            , AviaTicketsViewModel viewModel)
        {
            _logger = loggerFactory.CreateLogger<CitiesListCreatingWorkflow>();   

            _viewModel = viewModel;            

            _scheduler = schedulerFactory.Create().Do(() => { _viewModel.Cities = GetCities(); });
                            
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

        public List<Cities>? GetCities()
        { 
            
                var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
                request.Run();
                var response = request.Response;
                var info = JsonConvert.DeserializeObject<List<Cities>>(response);
            int a = 2; int b = 2; int c = a / b;
                return info;            
   
        }
    }
}
