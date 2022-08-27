using AviaTickets.Converters;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace AviaTickets.Processes
{
    public class CitiesListCreatingWorkflow : ICitiesListCreatingWorkflow
    {
        private ILogger<CitiesListCreatingWorkflow> _logger;
        private ISchedulerFactory _scheduler;        
        private IView _viewModel;        
        private CitiesConverter _converter;
        public string WorkflowType { get; set; } = "CITIES_LIST_CREATING";        

        public CitiesListCreatingWorkflow(ILogger<CitiesListCreatingWorkflow> logger
            , ISchedulerFactory schedulerFactory            
            , IView viewModel            
            , CitiesConverter converter)
        {
            _logger = logger;  
            _viewModel = viewModel;
            _converter = converter;

            _scheduler = schedulerFactory.Create()
                                         .Do(GetCities);
                            
        }      

        public (bool,object?) Start()
        {    
            var result = _scheduler.Start();
            return (result, null);
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
