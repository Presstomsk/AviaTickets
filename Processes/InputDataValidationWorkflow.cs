using AviaTickets.Processes.Abstractions;
using AviaTickets.Processes.AllProcessesList;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.ViewModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace AviaTickets.Processes
{
    public class InputDataValidationWorkflow : IInputDataValidationWorkflow
    {
        private ILogger<InputDataValidationWorkflow> _logger;
        private ISchedulerFactory _scheduler;
        private AbstractValidator<AviaTicketsViewModel> _validator;
        private AviaTicketsViewModel _view;

        public string WorkflowType { get; set; } = "INPUT_DATA_VALIDATION";
        public InputDataValidationWorkflow(ILogger<InputDataValidationWorkflow> logger
                                           , ISchedulerFactory schedulerFactory
                                           , AbstractValidator<AviaTicketsViewModel> validator
                                           , AviaTicketsViewModel view)
        {
            _logger = logger;           
            _validator = validator;
            _view = view;

            _scheduler = schedulerFactory.Create()
                                         .Do(Validate);
        }

        public void Start()
        {
            try
            {
                _logger.LogInformation($"PROCESS: {WorkflowType} STATUS: {STATUS.START}");
                _scheduler.Start();
                _logger.LogInformation($"PROCESS: {WorkflowType} STATUS: {STATUS.DONE}");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"PROCESS: {WorkflowType} STATUS: {STATUS.ERROR}", ex.Message);
            }
        }

        private void Validate()
        {
            var result = _validator.Validate(_view);

            if (result.IsValid)
            {
                _logger?.LogError($"RESULT_OF_VALIDATION_INPUT_DATA , STATUS: {STATUS.DONE}");
                _view.WithoutValidationErrors = true;
                return;
            }

            foreach (var error in result.Errors)
            {
                _logger?.LogError($"RESULT_OF_VALIDATION_INPUT_DATA , STATUS: {STATUS.ERROR}, {error.ErrorMessage}");
                MessageBox.Show(error.ErrorMessage, "Error input data", MessageBoxButton.OK, MessageBoxImage.Error);
                _view.WithoutValidationErrors = false;
                return;
            }

        }
    }
}
