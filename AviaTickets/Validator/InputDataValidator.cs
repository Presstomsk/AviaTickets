﻿using AviaTickets.ViewModel.Absractions;
using FluentValidation;

namespace AviaTickets.Validator
{
    public class InputDataValidator : AbstractValidator<IView>
    {
        private IView _view;
        public InputDataValidator(IView view)
        {
            _view = view;

            RuleFor(x => x.Cities).NotEmpty().WithMessage("Ошибка подключения к серверу!");

            RuleFor(x => x.DepCity).NotEmpty().WithMessage("Введите пункт отправления!")
                                 .Must(CheckCities).WithMessage("Введите корректный пункт отправления!");

            RuleFor(x => x.ArrCity).NotEmpty().WithMessage("Введите пункт прибытия!")
                                 .Must(CheckCities).WithMessage("Введите корректный пункт прибытия!");
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

   