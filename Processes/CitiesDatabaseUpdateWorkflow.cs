using AviaTickets.Converters;
using AviaTickets.DB.Abstractions;
using AviaTickets.Models;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using AviaTickets.Scheduler.Abstractions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AviaTickets.Processes
{
    public class CitiesDatabaseUpdateWorkflow : ICitiesDatabaseUpdateWorkflow
    {
        private IContextFactory _contextFactory;
        private ISchedulerFactory _scheduler;
        private CitiesConverter _converter;

        private DateTime? _updateDate = default;
        private bool _needUpdate = false;
        public string WorkflowType { get; set; } = "CITIES_DATABASE_UPDATE_WORKFLOW";

        public CitiesDatabaseUpdateWorkflow(IContextFactory contextFactory
                                            , ISchedulerFactory schedulerFactory
                                            , CitiesConverter converter)
        {
            _contextFactory = contextFactory;
            _converter = converter;

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(GetUpdateDBDate)
                                         .Do(NeedUpdate)
                                         .Do(CleareDB)
                                         .Do(UpdateDB);
        }

        public (bool, object?) Start()
        {
            var result = _scheduler.Start();
            return (result, null);
        }
        public void GetUpdateDBDate()
        {
            using (var db = _contextFactory.CreateContext())
            {
                var city = db.Cities.Select(x => x).ToList();
                if (city.Count > 0)
                {
                    _updateDate = city.OrderBy(x => x.UpdateDate).First().UpdateDate;
                }
            }
        }

        public void NeedUpdate()
        {
            _needUpdate = (_updateDate == default) ? true : (-((new DateTime(Int32.Parse(_updateDate?.ToString("yyyy")), Int32.Parse(_updateDate?.ToString("MM")), Int32.Parse(_updateDate?.ToString("dd"))) - DateTime.Today).TotalDays) > 7);
        }

        public async void CleareDB()
        {
            if (_needUpdate)
            {
                using (var context = _contextFactory.CreateContext())
                {
                    context.Cities.Select(x => x).ToList().ForEach(x => { context.Remove(x); });
                    await context.SaveChangesAsync();
                }
            }
        }

        public async void UpdateDB()
        {
            if (_needUpdate)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(_converter);

                var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
                request.Run();
                var response = request.Response;
                var info = JsonConvert.DeserializeObject<List<Cities>>(response, settings);

                using (var context = _contextFactory.CreateContext())
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {                              
                                await context.BulkInsertAsync(info);

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception(ex.Message);
                            }
                        }
                    });
                }
            };
        }
    }
}
