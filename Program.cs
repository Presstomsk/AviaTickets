using AviaTickets.Controller;
using AviaTickets.Converters;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Validator;
using AviaTickets.ViewModel;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace AviaTickets
{    public class Program
    {
        private ServiceProvider _serviceProvider;       

        public Program()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var serilog = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            _serviceProvider = new ServiceCollection()                                    
                                    .AddSingleton(configuration)
                                    .AddLogging((config) => config.AddSerilog(serilog))
                                    .AddSingleton<MainWindow>()                                   
                                    .AddSingleton<IView, View>()                                    
                                    .AddSingleton<CitiesConverter>()
                                    .AddSingleton<TicketConverter>()
                                    .AddSingleton<ICitiesListCreatingWorkflow, CitiesListCreatingWorkflow>()
                                    .AddTransient<IInputDataValidationWorkflow, InputDataValidationWorkflow>()
                                    .AddTransient<IAviaTicketsGetWorkflow, AviaTicketsGetWorkflow>() 
                                    .AddSingleton<ISchedulerFactory, SchedulerFactory>()
                                    .AddTransient<AbstractValidator<IView>, InputDataValidator>()
                                    .AddTransient<ICities, Cities>()
                                    .AddTransient<ITicket, Result>()                           
                                    .BuildServiceProvider();

            new MainController(_serviceProvider);

        }
    }
}
