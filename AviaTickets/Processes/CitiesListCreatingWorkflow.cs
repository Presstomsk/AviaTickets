using AviaTickets.DB.Abstractions;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Scheduler;
using System;
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

            _scheduler = schedulerFactory.Create()
                                         .Do(GetCities);
                                         
                            
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
            return _scheduler.Start();            
        }        

        public IMessage? GetCities(IMessage? message = default)
        {
            using (var context = _contextFactory.CreateContext())
            {
                _viewModel.Cities = context.Cities.Select(x => new Cities{ City = x.City, Code = x.Code}).ToList<ICities>();
            }

            return message;
        }

       
    }
}
