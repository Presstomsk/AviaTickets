using AviaTickets.Converters;
using AviaTickets.DB.Abstractions;
using AviaTickets.Models.Abstractions;
using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.HttpConnect;
using AviaTickets.Scheduler.Abstractions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AviaTickets.Processes
{
    public class CitiesDatabaseUpdateWorkflow : ICitiesDatabaseUpdateWorkflow
    {
        private ILogger<ICitiesDatabaseUpdateWorkflow> _logger;
        private IContextFactory _contextFactory;
        private ISchedulerFactory _scheduler;
        private CitiesConverter _converter;

        private DateTime? _updateDate = default;
        private bool _needUpdate = false;

        private List<ICities>? _info;
        public string WorkflowType { get; set; } = "CITIES_DATABASE_UPDATE_WORKFLOW";

        public CitiesDatabaseUpdateWorkflow(IContextFactory contextFactory
                                            , ISchedulerFactory schedulerFactory
                                            , CitiesConverter converter
                                            , ILogger<ICitiesDatabaseUpdateWorkflow> logger)
        {
            _contextFactory = contextFactory;
            _converter = converter;
            _logger = logger;

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(GetUpdateDBDate)
                                         .Do(NeedUpdate)
                                         .Do(CleareDB)
                                         .Do(RequestCities)
                                         .Do(UpdateDB);
        }

        public Statuses.Result Start()
        {
            var result = _scheduler.Start();
            return  new Statuses.Result { Success = result, Content = null }; 
        }
        public void GetUpdateDBDate()
        {
            using (var db = _contextFactory.CreateContext())
            {
                var city = db.Cities.Select(x => x).ToList();
                if (city.Count > 0)
                {
                    _updateDate = city.OrderBy(x => x.UpdateDate).First().UpdateDate;
                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {WorkflowType}, STEP[1] : GetUpdateDBDate, Последнее обновление БД было {_updateDate?.ToString("dd.MM.yyyy")}");
                }
                else _logger.LogInformation($"[{DateTime.Now}] PROCESS : {WorkflowType}, STEP[1] : GetUpdateDBDate, БД не заполнена");
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
                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {WorkflowType}, STEP[3] : CleareDB, База данных очищена.");
                    await context.SaveChangesAsync();
                }
            }
        }

        public void RequestCities()
        {
            if(_needUpdate)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(_converter);

                var request = new GetRequest("http://api.travelpayouts.com/data/ru/cities.json");
                request.Run();
                var response = request.Response;
                _info = JsonConvert.DeserializeObject<List<ICities>>(response, settings);
                _logger.LogInformation($"[{DateTime.Now}] PROCESS : {WorkflowType}, STEP[4] : RequestCities, Данные загружены  - { _info?.Count} элементов ");
                
            }
        }

        public async void UpdateDB()
        {
            if (_needUpdate)
            {
                using (var context = _contextFactory.CreateContext())
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

                    _logger.LogInformation($"[{DateTime.Now}] PROCESS : {WorkflowType}, STEP[5] : UpdateDB, База данных обновлена.");
                }
            };
        }
    }
}
