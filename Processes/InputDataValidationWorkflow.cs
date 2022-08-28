﻿using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace AviaTickets.Processes
{
    public class InputDataValidationWorkflow : IInputDataValidationWorkflow
    {
        private ILogger<InputDataValidationWorkflow> _logger;
        private ISchedulerFactory _scheduler;
        private AbstractValidator<IView> _validator;
        private IView _view;
        

        public string WorkflowType { get; set; } = "INPUT_DATA_VALIDATION";
        public InputDataValidationWorkflow(ILogger<InputDataValidationWorkflow> logger
                                           , ISchedulerFactory schedulerFactory
                                           , AbstractValidator<IView> validator
                                           , IView view)
        {
            _logger = logger;           
            _validator = validator;
            _view = view;

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(Validate);
        }

        public (bool, object?) Start()
        {
            var result = _scheduler.Start();
            return (result, null);
        }

        private void Validate()
        {
            var result = _validator.Validate(_view);

            if (result.IsValid) return;

            foreach (var error in result.Errors)
            {                
                MessageBox.Show(error.ErrorMessage, "Error input data", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new System.Exception(error.ErrorMessage);               
            }

        }
    }
}
