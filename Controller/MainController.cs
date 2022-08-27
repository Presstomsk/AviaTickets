using AviaTickets.Processes.Abstractions;
using AviaTickets.ViewModel.Absractions;
using Microsoft.Extensions.DependencyInjection;


namespace AviaTickets.Controller
{    
    public class MainController 
    {
        private IView? _view;
        private MainWindow? _mainWindow;
        private ServiceProvider _serviceProvider;
        public MainController (ServiceProvider serviceProvider)
        {
            _mainWindow = serviceProvider.GetService<MainWindow>();
            _view = serviceProvider.GetService<IView>();            
            if (_mainWindow != default) _mainWindow.DataContext = _view;
            _mainWindow?.Show();

            _serviceProvider = serviceProvider;
            _serviceProvider.GetService<ICitiesListCreatingWorkflow>()?.Start();

            if (_view != default)
            {
                _view.SearchTickets += View_SearchTickets;
            }           
        }

        private void View_SearchTickets()
        {            
            _serviceProvider.GetService<IInputDataValidationWorkflow>()?.Start();
            if (_view.WithoutValidationErrors) _serviceProvider.GetService<IAviaTicketsGetWorkflow>()?.Start();            
        }
    }
}
