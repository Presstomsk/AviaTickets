using AviaTickets.DB;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Microsoft.EntityFrameworkCore;
using Scheduler;
using System;
using System.Linq;

namespace AviaTickets.Processes
{
    public class CitiesListCreatingWorkflow : ICitiesListCreatingWorkflow
    {
        private IDbContextFactory<MainContext> _contextFactory;
        private ISchedulerFactory _scheduler;        
        private IView _viewModel;       
        
        public string WorkflowType { get; set; } = "CITIES_LIST_CREATING_WORKFLOW";        

        public CitiesListCreatingWorkflow(ISchedulerFactory schedulerFactory            
                                          , IView viewModel                                       
                                          , IDbContextFactory<MainContext> contextFactory)
        {           
            _viewModel = viewModel;            
            _contextFactory = contextFactory;

            _scheduler = schedulerFactory.Create()
                                         .Do(GetCities);
                                         
                            
        }

        public IMessage? Start(IMessage? msg)
        {            
            return Start();
        }

        public IMessage? Start()
        {
            return _scheduler.Start();            
        }        

        public IMessage? GetCities(IMessage? message = default)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                _viewModel.Cities = context.Cities.Select(x => new Cities{ City = x.City, Code = x.Code}).ToList<ICities>();
            }

            return message;
        }

       
    }
}
