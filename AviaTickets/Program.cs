﻿using AviaTickets.Controller;
using AviaTickets.Converters;
using AviaTickets.DB;
using AviaTickets.Models;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Validator;
using AviaTickets.ViewModel;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler;
using Serilog;
using Splash;
using System;
using System.Windows;

namespace AviaTickets
{    public class Program
    {
        private ServiceProvider _serviceProvider;       

        public Program()
        {
            try
            {
                var mainWindow = new MainWindow();

                using (var loadingVisualiser = new SplashWindow("loading..."))
                {
                    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                    var serilog = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();                    

                    _serviceProvider = new ServiceCollection()
                                            .AddSingleton(configuration)
                                            .AddLogging((config) => config.AddSerilog(serilog))
                                            .AddSingleton(mainWindow)
                                            .AddSingleton<IView, View>()
                                            .AddSingleton<CitiesConverter>()
                                            .AddSingleton<TicketConverter>()                                            
                                            .AddDbContextFactory<MainContext>()                                            
                                            .AddSingleton<ICitiesListCreatingWorkflow, CitiesListCreatingWorkflow>()
                                            .AddTransient<IInputDataValidationWorkflow, InputDataValidationWorkflow>()
                                            .AddTransient<IAviaTicketsGetWorkflow, AviaTicketsGetWorkflow>()
                                            .AddTransient<IOpenTicketLinkWorkflow, OpenTicketLinkWorkflow>()
                                            .AddTransient<ITicketsCreatedWorkflow, TicketsCreatedWorkflow>()
                                            .AddTransient<IAddTicketsIntoViewWorkflow, AddTicketsIntoViewWorkflow>()
                                            .AddSingleton<ICitiesDatabaseUpdateWorkflow,CitiesDatabaseUpdateWorkflow>()                                            
                                            .AddTransient<ISchedulerFactory, SchedulerFactory>()
                                            .AddTransient<AbstractValidator<IView>, InputDataValidator>()                                            
                                            .AddTransient<ICities, Cities>()
                                            .AddTransient<ITicket, Result>()
                                            .BuildServiceProvider();

                    using (var db = _serviceProvider.GetService<IDbContextFactory<MainContext>>()?.CreateDbContext()) 
                    {                        
                        db?.Database.Migrate();
                    }                

                    new MainController(_serviceProvider);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
                Application.Current?.Shutdown();
                return;
            }

        }
    }
}
