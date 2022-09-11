﻿using AviaTickets.DB.Abstractions;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.ViewModel.Absractions;
using System.Linq;

namespace AviaTickets.Processes
{
    public class CitiesListCreatingWorkflow : ICitiesListCreatingWorkflow
    {
        private IContextFactory _contextFactory;
        private ISchedulerFactory _scheduler;        
        private IView _viewModel;       
        
        public string WorkflowType { get; set; } = "CITIES_LIST_CREATING_WORKFLOW";        

        public CitiesListCreatingWorkflow(ISchedulerFactory schedulerFactory            
                                          , IView viewModel                                       
                                          , IContextFactory contextFactory)
        {           
            _viewModel = viewModel;            
            _contextFactory = contextFactory;

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(GetCities);
                            
        }      

        public Statuses.Result Start()
        {    
            var result = _scheduler.Start();
            return new Statuses.Result { Success = result, Content = null };
        }

        public void GetCities()
        {
            using (var context = _contextFactory.CreateContext())
            {
                _viewModel.Cities = context.Cities.Select(x => new Cities{ City = x.City, Code = x.Code}).ToList<ICities>();
            }
        }
    }
}
