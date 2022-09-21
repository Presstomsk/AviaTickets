using AviaTickets.Converters;
using AviaTickets.DB;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AviaTickets.Processes
{
    public class CitiesDatabaseUpdateWorkflow : ICitiesDatabaseUpdateWorkflow
    {
        private ILogger<ICitiesDatabaseUpdateWorkflow> _logger;
        private IDbContextFactory<MainContext> _contextFactory;
        private ISchedulerFactory _scheduler;
        private CitiesConverter _converter;

        private DateTime? _updateDate = default;
        private bool _needUpdate = false;
        private List<ICities>? _info;

        public DateTime? UpdateDate { get { return _updateDate; } }
        public bool IsNeedUpdate { get { return _needUpdate; } }
        public List<ICities>? Info { get { return _info; } }
        public string WorkflowType { get; set; } = "CITIES_DATABASE_UPDATE_WORKFLOW";

        public CitiesDatabaseUpdateWorkflow(IDbContextFactory<MainContext> contextFactory
                                            , ISchedulerFactory schedulerFactory
                                            , CitiesConverter converter
                                            , ILogger<ICitiesDatabaseUpdateWorkflow> logger)
        {
            _contextFactory = contextFactory;
            _converter = converter;
            _logger = logger;

            _scheduler = schedulerFactory.Create()
                                         .Do(GetUpdateDBDate)
                                         .Do(NeedUpdate)
                                         .Do(CleareDB)
                                         .Do(RequestCities)
                                         .Do(UpdateDB);
                                         
        }

        public IMessage? Start(IMessage? msg = default)
        {            
            return Start();
        }

        public IMessage? Start()
        {
            return _scheduler.Start();            
        }
        public IMessage? GetUpdateDBDate(IMessage? message = default)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                var city = db.Cities.Select(x => new {UpdateDate = x.UpdateDate}).ToList();
                if (city.Count > 0)
                {
                    _updateDate = city.OrderBy(x => x.UpdateDate).First().UpdateDate;
                    _logger.LogInformation($"[{DateTime.Now}] Последнее обновление БД было {_updateDate?.ToString("dd.MM.yyyy")}");
                }
                else _logger.LogInformation($"[{DateTime.Now}] БД не заполнена");
            }

            return message;
        }

        public IMessage? NeedUpdate(IMessage? message = default)
        {
            var delta =(int?) -_updateDate?.Subtract(DateTime.Now).TotalDays;
            _needUpdate = (_updateDate == default) ? true : delta > 30;

            return message;            
        }

        public IMessage? CleareDB(IMessage? message = default)
        {
            CleareDataBase();
            return message;
        }

        public async void CleareDataBase()
        {
            if (_needUpdate)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    context.Cities.Select(x => x).ToList().ForEach(x => { context.Remove(x); });
                    _logger.LogInformation($"[{DateTime.Now}] База данных очищена.");
                    await context.SaveChangesAsync();
                }
            }
        }

        public IMessage? RequestCities(IMessage? message = default)
        {
            if(_needUpdate)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(_converter);

                var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
                request.Run();
                var response = request.Response;
                _info = JsonConvert.DeserializeObject<List<ICities>>(response, settings);
                _logger.LogInformation($"[{DateTime.Now}]  Данные загружены  - { _info?.Count} элементов ");
                
            }

            return message;
        }

        public IMessage? UpdateDB(IMessage? message = default)
        {
            UpdateDataBase();

            return message;
        }

        public async void UpdateDataBase()
        {
            if (_needUpdate)
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var strategy = context.Database.CreateExecutionStrategy();
                    await strategy.ExecuteAsync(async () =>
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {

                                await context.BulkInsertAsync(_info);

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception(ex.Message);
                            }
                        }
                    });

                    _logger.LogInformation($"[{DateTime.Now}] База данных обновлена.");
                }
            };
        }
    }
}
