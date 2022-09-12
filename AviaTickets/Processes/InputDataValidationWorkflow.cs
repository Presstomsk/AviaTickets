using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
using Scheduler;
using System;
using System.Windows;

namespace AviaTickets.Processes
{
    public class InputDataValidationWorkflow : IInputDataValidationWorkflow
    {        
        private ISchedulerFactory<IOut> _scheduler;
        private AbstractValidator<IView> _validator;
        private IView _view;

        private bool _validateStatus = false;

        public string WorkflowType { get; set; } = "INPUT_DATA_VALIDATION";
        public InputDataValidationWorkflow( ISchedulerFactory<IOut> schedulerFactory
                                           , AbstractValidator<IView> validator
                                           , IView view)
        {                    
            _validator = validator;
            _view = view;

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(Validate)
                                         .Build();
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
            var answer = _scheduler.StartProcess();
            return new Msg.Message(answer.Item1, null, null, answer.Item2, _validateStatus);
        }        

        private void Validate()
        {
            var result = _validator.Validate(_view);

            if (result.IsValid) return;

            foreach (var error in result.Errors)
            {                
                MessageBox.Show(error.ErrorMessage, "Error input data", MessageBoxButton.OK, MessageBoxImage.Error);
                _validateStatus = true;
                throw new Exception(error.ErrorMessage);               
            }

        }

        
    }
}
