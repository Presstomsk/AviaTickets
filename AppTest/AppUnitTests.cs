using AviaTickets;
using AviaTickets.Converters;
using AviaTickets.DB;
using AviaTickets.DB.Abstractions;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AppTest
{
    [TestFixture]
    public class Tests
    {

        private IConfigurationRoot _configuration;
        private Serilog.ILogger _serilog;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("testAppsettings.json").Build();
            _serilog = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();
            _serviceProvider = new ServiceCollection()
                                           .AddSingleton(_configuration)
                                           .AddLogging((config) => config.AddSerilog(_serilog))                                           
                                           .AddSingleton<CitiesConverter>()
                                           .AddSingleton<TicketConverter>()
                                           .AddSingleton<IView, View>()
                                           .AddSingleton<IContextFactory, TestContextFactory>()
                                           .AddSingleton<ICitiesDatabaseUpdateWorkflow, CitiesDatabaseUpdateWorkflow>()
                                           .AddSingleton<ICitiesListCreatingWorkflow, CitiesListCreatingWorkflow>()
                                           .AddTransient<IInputDataValidationWorkflow, InputDataValidationWorkflow>()
                                           .AddTransient<IAviaTicketsGetWorkflow, AviaTicketsGetWorkflow>()                                           
                                           .AddTransient<AbstractValidator<IView>, InputDataValidator>()
                                           .AddSingleton<ISchedulerFactory, SchedulerFactory>()
                                           .AddTransient<ICities, Cities>()
                                           .BuildServiceProvider();
            using (var db = new TestMainContext())
            {
                db.Database.Migrate();
                db.Cities.Select(x => x).ToList().ForEach(x => { db.Remove(x); });
                db.SaveChanges();
            }
        }

        [Test]
        public void CitiesDatabaseUpdateWorkflow_NoDB_CreateDB_TrueResult()
        {   
            var process = _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>();
            var result = process.Start();

            var numberElementsInDB = 0;

            using (var db = new TestMainContext())
            {               
               numberElementsInDB = db.Cities.Select(x => x).ToList().Count;                
            }

            Assert.IsTrue(process.UpdateDate == default);
            Assert.IsTrue(process.IsNeedUpdate);
            Assert.IsTrue(process.Info.Count > 0);
            Assert.IsTrue(numberElementsInDB == process.Info.Count);
            Assert.IsTrue(result.Success);
        }
        
        [TestCase("TestCity", "TEST", -30)]
        public void CitiesDatabaseUpdateWorkflow_DBCreatedLessThen30Days_CreatedDBNoUpdate_TrueResult(string city, string code, int daysAgo)
        {
            var testCity = new Cities
            {
                City = city,
                Code = code,
                UpdateDate = DateTime.Now.AddDays(daysAgo)
            };

            using (var db = new TestMainContext())
            {
                db.Cities.Add(testCity);
                db.SaveChanges();
            }

            var process = _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>();
            var result = process.Start();

            var numberElementsInDB = 0;
            var elementInTestDB = new Cities();

            using (var db = new TestMainContext())
            {
                numberElementsInDB = db.Cities.Select(x => x).ToList().Count;
                elementInTestDB = db.Cities.First();
            }

            Assert.IsTrue(process.UpdateDate == testCity.UpdateDate);
            Assert.IsFalse(process.IsNeedUpdate);
            Assert.IsTrue(process.Info == default);
            Assert.IsTrue(numberElementsInDB == 1);
            Assert.IsTrue(elementInTestDB.City == city 
                         && elementInTestDB.Code == code);
            Assert.IsTrue(result.Success);
        }

        
        [TestCase("TestCity", "TEST", -31)]
        public void CitiesDatabaseUpdateWorkflow_DBCreatedMoreThen30Days_CreatedDBUpdate_TrueResult(string city, string code, int daysAgo)
        {
            var testCity = new Cities
            {
                City = city,
                Code = code,
                UpdateDate = DateTime.Now.AddDays(daysAgo)
            };

            using (var db = new TestMainContext())
            {
                db.Cities.Add(testCity);
                db.SaveChanges();
            }

            var process = _serviceProvider.GetService<ICitiesDatabaseUpdateWorkflow>();            
            var result = process.Start();

            var numberElementsInDB = 0;
            var elementInTestDB = new Cities();

            using (var db = new TestMainContext())
            {
                numberElementsInDB = db.Cities.Select(x => x).ToList().Count;
                elementInTestDB = db.Cities.First();
            }

            Assert.IsTrue(process.UpdateDate == testCity.UpdateDate);
            Assert.IsTrue(process.IsNeedUpdate);
            Assert.IsTrue(process.Info.Count > 0);
            Assert.IsTrue(numberElementsInDB == process.Info.Count);
            Assert.IsTrue(result.Success);
        }

        [TestCase("TestCity", "TEST")]
        public void CitiesListCreatingWorkflow_GetListOfCitiesFromDB_TrueResult(string city, string code)
        {
            var testCity = new Cities
            {
                City = city,
                Code = code,
                UpdateDate = DateTime.Now
            };

            using (var db = new TestMainContext())
            {
                db.Cities.Add(testCity);
                db.SaveChanges();
            }

            var process = _serviceProvider.GetService<ICitiesListCreatingWorkflow>();           
            var result = process.Start();

            var numberElementsInDB = 0;

            using (var db = new TestMainContext())
            {
                numberElementsInDB = db.Cities.Select(x => x).ToList().Count;
            }

            var view = _serviceProvider.GetService<IView>();
            var elementInListOfCities = view.Cities.First();

            Assert.IsTrue(numberElementsInDB == view.Cities.Count
                          && view.Cities.Count == 1);
            Assert.IsTrue(elementInListOfCities.City == city
                          && elementInListOfCities.Code == code);
            Assert.IsTrue(result.Success);
        }

        [TestCase("Москва", "Новосибирск")]
        public void InputDataValidationWorkflow_ValidationCitiesList_FalseResult(string depCity, string arrCity)
        {
            var view = _serviceProvider.GetService<IView>();
            view.DepCity = depCity;
            view.ArrCity = arrCity;
            view.Cities = default;

            AbstractValidator<IView> validator = new InputDataValidator(view);
            var result = validator.Validate(view);

            var error = result.Errors.First().ErrorMessage;

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(error == "Ошибка подключения к серверу!");

        }

        [TestCase("", "Новосибирск")]
        [TestCase("Москва", "")]
        [TestCase("Moscow", "Новосибирск")]
        [TestCase("Москва", "Novosibirsk")]
        public void InputDataValidationWorkflow_ValidationDepAndArrCities_FalseResult(string depCity, string arrCity)
        {
            var cities = new List<ICities>();
            cities.Add(new Cities { City = "Москва", Code = "MOW" });
            cities.Add(new Cities { City = "Новосибирск", Code = "OVB" });

            var view = _serviceProvider.GetService<IView>();
            view.DepCity = depCity;
            view.ArrCity = arrCity;
            view.Cities = cities;

            AbstractValidator<IView> validator = new InputDataValidator(view);
            var result = validator.Validate(view);

            var error = result.Errors.First().ErrorMessage;

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(!string.IsNullOrEmpty(error));
        }

        [TestCase("Москва", "Новосибирск")]
        [TestCase("MOW", "OVB")]
        [TestCase("Москва", "OVB")]
        [TestCase("MOW", "Новосибирск")]
        public void InputDataValidationWorkflow_ValidationDepAndArrCities_TrueResult(string depCity, string arrCity)
        {
            var cities = new List<ICities>();
            cities.Add(new Cities { City = "Москва", Code = "MOW" });
            cities.Add(new Cities { City = "Новосибирск", Code = "OVB" });

            var view = _serviceProvider.GetService<IView>();
            view.DepCity = depCity;
            view.ArrCity = arrCity;
            view.Cities = cities;

            AbstractValidator<IView> validator = new InputDataValidator(view);
            var result = validator.Validate(view);

            var error = result.Errors.FirstOrDefault()?.ErrorMessage;

            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(string.IsNullOrEmpty(error));
        }

        [TestCase("Москва", "Новосибирск", true, true, false)]
        [TestCase("MOW", "OVB", false, true, true)]
        [TestCase("Москва", "OVB", true, false, true)]
        [TestCase("MOW", "Новосибирск", true, true, true)]
        public void AviaTicketsGetWorkflow_GetListOfTickets_TrueResult(string depCity
                                                                               , string arrCity
                                                                               , bool oneWayTicket
                                                                               , bool returnTicket
                                                                               , bool wayWithTransferTicket)
        {
            var view = _serviceProvider.GetService<IView>();
            view.ArrDate = DateTime.Now;
            view.DepDate = DateTime.Now;
            view.DepCity = depCity;
            view.ArrCity = arrCity;
            view.OneWayTicket = oneWayTicket;
            view.ReturnTicket = returnTicket;
            view.WayWithTransferTicket = wayWithTransferTicket;

            var cities = new List<ICities>();
            cities.Add(new Cities { City = "Москва", Code = "MOW" });
            cities.Add(new Cities { City = "Новосибирск", Code = "OVB" });
            view.Cities = cities;

            var process = _serviceProvider.GetService<IAviaTicketsGetWorkflow>();            
            var result = process.Start();
            var data = result.Content;

            Assert.IsTrue(data != null);
            Assert.IsTrue((data as List<Data>).Count > 0);
            Assert.IsTrue(result.Success);
        }

        [TearDown]
        public void TearDown()
        {
            _configuration = default;
            _serilog = default;
            _serviceProvider = default;           
        }

        public class TestMainContext : MainContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite($"Data Source = testmydb.db");
            }
        }

        public class TestContextFactory : IContextFactory
        {
            public MainContext CreateContext()
            {
                return new TestMainContext();
            }
        }        
    }
}