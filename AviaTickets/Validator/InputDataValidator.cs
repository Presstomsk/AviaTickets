using AviaTickets.ViewModel.Absractions;
using FluentValidation;

namespace AviaTickets.Validator
{
    public class InputDataValidator : AbstractValidator<IView>
    {
        private IView _view;
        public InputDataValidator(IView view)
        {
            _view = view;

            RuleFor(x => x.Cities).NotEmpty().WithMessage("Ошибка загрузки данных!");

            RuleFor(x => x.DepCity).NotEmpty().WithMessage("Введите пункт отправления!")
                                 .Must(CheckCities).WithMessage("Введите корректный пункт отправления!");

            RuleFor(x => x.ArrCity).NotEmpty().WithMessage("Введите пункт прибытия!")
                                 .Must(CheckCities).WithMessage("Введите корректный пункт прибытия!");            
            RuleFor(x => x.OneWayTicket).Must(x => x).WithMessage("Выберите тип билета!").When(x => !x.ReturnTicket);
           
        }       

        protected bool CheckCities(string depCity)
        {
            if (_view.Cities != null)
            {
                foreach (var city in _view.Cities)
                {
                    if (depCity == city.City || depCity == city.Code) return true;
                }   
                return false;
            }
            else return false;
            
        }       
    }
}

   
