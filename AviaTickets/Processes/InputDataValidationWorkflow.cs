﻿using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
using Scheduler;
using Scheduler.Message;
using System;
using System.Windows;

namespace AviaTickets.Processes
{
    public class InputDataValidationWorkflow : IInputDataValidationWorkflow
    {        
        private ISchedulerFactory _scheduler;
        private AbstractValidator<IView> _validator;
        private IView _view;        

        public string WorkflowType { get; set; } = "INPUT_DATA_VALIDATION";
        public InputDataValidationWorkflow( ISchedulerFactory schedulerFactory
                                           , AbstractValidator<IView> validator
                                           , IView view)
        {                    
            _validator = validator;
            _view = view;

            _scheduler = schedulerFactory.Create()
                                         .Do(Validate);
                                         
        }

        public IMessage? Start(IMessage? msg)
        {           
            return Start();
        }

        public IMessage? Start()
        {
            return _scheduler.Start();            
        }        

        private IMessage? Validate(IMessage? message = default)
        {
            var result = _validator.Validate(_view);

            if (result.IsValid) return message;

            foreach (var error in result.Errors)
            {                
                MessageBox.Show(error.ErrorMessage, "Error input data", MessageBoxButton.OK, MessageBoxImage.Error);                
                return new Message().SendError(MsgType.LogError, new Exception(error.ErrorMessage));
            }

            return new Message().SendError(MsgType.LogError, new Exception()); 
        }        
    }
}
