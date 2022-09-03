using AviaTickets.Processes.Abstractions;
using AviaTickets.Scheduler.Abstractions;
using AviaTickets.Statuses;
using AviaTickets.ViewModel.Absractions;
using FluentValidation;
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

            _scheduler = schedulerFactory.Create(WorkflowType)
                                         .Do(Validate);
        }

        public Result Start()
        {
            var result = _scheduler.Start();
            return new Result { Success = result, Content = null };
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
